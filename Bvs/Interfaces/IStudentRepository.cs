using Bvs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> GetStudentByIdAsync(int id);

        Task<IEnumerable<Student>> GetStudentsAsync();

        void AddStudent(Student student);

        void RemoveStudent(Student student);

        Task<bool> CheckMatrikelnummer(int matrikelNum);

        Task<bool> CheckBibNummer(int bibNum);

        Task<bool> SaveAllAsync();
    }
}
