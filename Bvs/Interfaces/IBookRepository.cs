using Bvs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetBooksAsync();

        Task<bool> IsBookIsbnExistsAsync(string isbn);

        Task<Book> GetBookByIdAsync(int id);

        void AddBook(Book book);

        void RemoveBook(Book book);

    }
}
