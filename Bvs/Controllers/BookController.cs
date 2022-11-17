using AutoMapper;
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
    public class BookController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IBookRepository _bookRebo;

        public BookController(DataContext context, IMapper mapper, IBookRepository bookRepository)
        {
            _context = context;
            _mapper = mapper;
            _bookRebo = bookRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return Ok(await _bookRebo.GetBooksAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            return Ok(await _bookRebo.GetBookByIdAsync(id));
        }

        [HttpPost("addBook")]
        public async Task<ActionResult> AddBook(BookDto bookDto)
        {
            if (await _bookRebo.IsBookIsbnExistsAsync(bookDto.Isbn))
            {
                return BadRequest($"ISBN: {bookDto.Isbn} ist bereits existiert!");
            }

            Book book = _mapper.Map<Book>(bookDto);
            book.Verfuegbar = book.Anzahl;
            book.B_foto = "";

            _bookRebo.AddBook(book);
            await _context.SaveChangesAsync();

            await BookCount();
            await _context.SaveChangesAsync();

            return Ok(book);
        }

        [HttpPut("editBook")]
        public async Task<ActionResult<Book>> EditBook(BookDto bk)
        {
            Book book = await _bookRebo.GetBookByIdAsync(bk.Id);

            if (book == null)
            {
                return NotFound($"Book mit Id= {bk.Id} wurde nicht gefunden.");
            }

            if (await _bookRebo.IsBookIsbnExistsAsync(bk.Isbn) && book.Isbn != bk.Isbn)
            {
                return BadRequest($"ISBN: { bk.Isbn} ist bereits existiert!");
            }

            // changing the number of a book also changes the available of that book

            if (bk.Anzahl > book.Anzahl)
            {
                int temp = bk.Anzahl - book.Anzahl;
                book.Verfuegbar += temp;
            }
            else if (bk.Anzahl < book.Anzahl)
            {
                int temp = book.Anzahl - bk.Anzahl;
                book.Verfuegbar -= temp;
            }

            _mapper.Map(bk, book);

            await _context.SaveChangesAsync();
            return Ok(book);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            Book book = await _bookRebo.GetBookByIdAsync(id);

            if (book == null)
            {
                return NotFound($"Book mit {id} wurde nicht gefunden.");
            }

            bool isBorrowed = await _context.Borrow.AnyAsync(x => x.Books == book);

            if (isBorrowed)
            {
                return BadRequest("Dieses Buch ist noch ausgeliehen!");
            }

            _bookRebo.RemoveBook(book);
            await _context.SaveChangesAsync();

            await BookCount();
            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task BookCount()
        {
            var booksCount = await _context.Book.Select(x=>x.Anzahl).SumAsync();
            var overView = await _context.NumberOverview.FirstAsync();
            overView.AnzahlBook = booksCount;
        }
    }
}
