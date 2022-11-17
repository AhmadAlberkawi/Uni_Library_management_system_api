using Bvs.Entities;
using Bvs_API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.Data
{
    public class BookRepository : IBookRepository
    {
        private readonly DataContext _context;

        public BookRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            return await _context.Book.ToListAsync();
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            return await _context.Book.FindAsync(id);
        }

        public async Task<bool> IsBookIsbnExistsAsync(string isbn)
        {
            return await _context.Book.AnyAsync(x => x.Isbn.Equals(isbn));
        }

        public void AddBook(Book book)
        {
            _context.Book.Add(book);
        }

        public void RemoveBook(Book book)
        {
            _context.Book.Remove(book);
        }
    }
}
