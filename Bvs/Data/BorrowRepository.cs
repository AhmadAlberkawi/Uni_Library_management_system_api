using Bvs.Entities;
using Bvs_API.DTOs;
using Bvs_API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.Data
{
    public class BorrowRepository : IBorrowRepository
    {
        private readonly DataContext _context;
        public BorrowRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<BorrowListDto>> GetBorrowsAsync()
        {
            var BookBorrowList = from st in _context.Student
                                 from bk in _context.Book
                                 from br in _context.Borrow
                                 where br.Books == bk && br.students == st
                                 orderby bk.Title
                                 select new BorrowListDto(
                                     br.Id,
                                     bk.B_foto,
                                     bk.Title,
                                     bk.Isbn,
                                     bk.Verlag,
                                     bk.Autor,
                                     st.Name,
                                     br.BorrowedUntil,
                                     br.GetremainingDays()
                                 );

            return await BookBorrowList.ToListAsync();

            //return Ok(await _context.Borrow.Include(x => x.Books).Include(x => x.students).
            //   ProjectTo<BorrowListDto>(_mapper.ConfigurationProvider).ToListAsync());
        }

        public async Task<IEnumerable<BorrowOfStudentDto>> GetBorrowForStudentAsync(Student student)
        {
            var BookBorrowList = from st in _context.Student
                                 from bk in _context.Book
                                 from br in _context.Borrow
                                 where br.Books == bk && br.students == st && st == student
                                 orderby bk.Title
                                 select new BorrowOfStudentDto(
                                     br.Id,
                                     bk.B_foto,
                                     bk.Title,
                                     bk.Isbn,
                                     bk.Verlag,
                                     bk.Autor,
                                     br.BorrowedUntil,
                                     br.GetremainingDays()
                                 );

            return await BookBorrowList.ToListAsync();
        }

        public async Task<Borrow> GetBorrowByIdAsync(int id)
        {
            return await _context.Borrow.Include(x => x.Books)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> IsBookBorrowedAsync(Student student, Book book)
        {
            return await _context.Borrow.AnyAsync(x => x.students == student && x.Books == book);
        }

        public void AddBorrow(Borrow borrow)
        {
            _context.Borrow.Add(borrow);
        }

        public void RemoveBorrow(Borrow borrow)
        {
            _context.Borrow.Remove(borrow);
        }
        
    }
}
