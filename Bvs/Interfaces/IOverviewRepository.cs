using Bvs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.Interfaces
{
    public interface IOverviewRepository
    {
        Task<NumberOverview> GetOverviewAsync();

        Task AdminsCountAsync();

        Task StudentsCountAsync();

        Task BorrowsCountAsync();

        Task BooksCountAsync();
    }
}
