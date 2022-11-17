using Bvs.Entities;
using Bvs_API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.Data
{
    public class OverviewRepository: IOverviewRepository
    {
        private readonly DataContext _context;

        public OverviewRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<NumberOverview> GetOverviewAsync()
        {
            return await _context.NumberOverview.FirstAsync();
        }

        // Admin counter for overview Page

        public async Task AdminsCountAsync()
        {
            int adminsCount = await _context.Admin.CountAsync();
            var numberOverview = await _context.NumberOverview.FirstAsync();
            numberOverview.AnzahlAdmin = adminsCount;
        }

        public async Task StudentsCountAsync()
        {
            int studenstCount = await _context.Student.CountAsync();
            var numberOverview = await _context.NumberOverview.FirstAsync();
            numberOverview.AnzahlStudent = studenstCount;
        }

        public async Task BorrowsCountAsync()
        {
            int borrowsCount = await _context.Borrow.CountAsync();
            var numberOverview = await _context.NumberOverview.FirstAsync();
            numberOverview.AnzahlBorrow = borrowsCount;
        }

        public async Task BooksCountAsync()
        {
            int booksCount = await _context.Book.Select(x => x.Anzahl).SumAsync();
            var overView = await _context.NumberOverview.FirstAsync();
            overView.AnzahlBook = booksCount;
        }
    }
}
