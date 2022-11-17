using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bvs.Entities;
using Bvs_API.Data;
using Bvs_API.DTOs;
using Bvs_API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.Controllers
{
    [Authorize]
    public class BorrowController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IBorrowRepository _borrowRebo;
        private readonly IMapper _mapper;

        public BorrowController(DataContext context, IBorrowRepository borrowRepository, IMapper mapper)
        {
            _context = context;
            _borrowRebo = borrowRepository;
            _mapper = mapper;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<BorrowListDto>>> BorrowList()
        {
            return Ok(await _borrowRebo.GetBorrowsAsync());
        }        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<BorrowOfStudentDto>>> BorrowForStudent(int id)
        {
            Student student = await _context.Student.FindAsync(id);

            if (student == null)
            {
                return BadRequest($"Student mit {id} wurde nicht gefunden");
            }

            return Ok(await _borrowRebo.GetBorrowForStudentAsync(student));
        }

        [HttpPost("addBorrow")]
        public async Task<ActionResult> AddBorrow(BorrowDto borrowDto)
        {
            Student student = await _context.Student.FindAsync(borrowDto.StudentId);

            Book book = await _context.Book.FindAsync(borrowDto.BookId);

            if (student == null || book == null)
            {
                return BadRequest("Student oder Buch wurde nicht gefunden!");
            }

            if (book.Verfuegbar == 0)
            {
                return BadRequest("Dieses Buch ist nicht verfügbar");
            }

            bool isBorrowed = await _context.Borrow.AnyAsync(x => x.students == student && x.Books == book);

            if (isBorrowed)
            {
                return BadRequest("Dieses Buch wurde schon von diesem Student ausgeliehen!");
            }

            Borrow borrow = new Borrow { students = student, Books = book };

            _borrowRebo.AddBorrow(borrow);

            await _context.SaveChangesAsync();

            book.Verfuegbar--;
            await BorrowCount();
            await _context.SaveChangesAsync();

            // BorrowListDto borrow1 = _mapper.Map<BorrowListDto>(borrow);
            BorrowListDto borrow1 = new BorrowListDto
            (
                borrow.Id,
                borrow.Books.B_foto,
                borrow.Books.Title,
                borrow.Books.Isbn,
                borrow.Books.Verlag,
                borrow.Books.Autor,
                borrow.students.Name,
                borrow.BorrowedUntil,
                borrow.GetremainingDays()
            );

            return Ok(borrow1);
        }        [HttpDelete("{id}")]        public async Task<ActionResult> DeleteBorrow(int id)
        {
            Borrow borrow = await _borrowRebo.GetBorrowByIdAsync(id);

            if (borrow == null)
            {
                return NotFound($"Ausleihe id {id} wurde nicht gefunden.");
            }

            borrow.Books.Verfuegbar++;

            _borrowRebo.RemoveBorrow(borrow);
            await _context.SaveChangesAsync();

            await BorrowCount();
            await _context.SaveChangesAsync();

            return Ok();
        }        private async Task BorrowCount()
        {
            var borrowCount = await _context.Borrow.CountAsync();
            var numberOverview = await _context.NumberOverview.FirstAsync();
            numberOverview.AnzahlBorrow = borrowCount;
        }    }
}
