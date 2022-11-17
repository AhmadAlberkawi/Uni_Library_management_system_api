using Bvs.Entities;
using Bvs_API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.Data
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DataContext _context;

        public StudentRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Student> GetStudentByIdAsync(int id)
        {
            return await _context.Student.FindAsync(id);
        }

        public async Task<IEnumerable<Student>> GetStudentsAsync()
        {
            return await _context.Student.ToListAsync();
        }

        public void AddStudent(Student student)
        {
            _context.Student.Add(student);
        }

        public void RemoveStudent(Student student)
        {
            _context.Student.Remove(student);
        }

        // check if "Matrikelnummer" is taken
        public async Task<bool> CheckMatrikelnummer(int matrikelNum)
        {
            return await _context.Student.AnyAsync(x => x.MatrikelNum == matrikelNum);
        }

        // check if "BibNummer" is taken
        public async Task<bool> CheckBibNummer(int bibNum)
        {
            return await _context.Student.AnyAsync(x => x.BibNum == bibNum);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
