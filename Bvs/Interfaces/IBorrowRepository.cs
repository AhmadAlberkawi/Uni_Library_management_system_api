using Bvs.Entities;
using Bvs_API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.Interfaces
{
    public interface IBorrowRepository
    {
        Task<IEnumerable<BorrowListDto>> GetBorrowsAsync();

        Task<IEnumerable<BorrowOfStudentDto>> GetBorrowForStudentAsync(Student student);

        Task<Borrow> GetBorrowByIdAsync(int id);

        Task<bool> IsBookBorrowedAsync(Student student, Book book);

        void AddBorrow(Borrow borrow);

        void RemoveBorrow(Borrow borrow);
    }
}
