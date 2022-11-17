using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.DTOs
{
    public class BorrowListDto
    {
        public int Id { get; set; }
        public string Foto { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string Verlag { get; set; }
        public string Autro { get; set; }
        public string StudentName { get; set; }
        public string BorrowedUntil { get; set; }
        public int RemainingDays { get; set; }

        public BorrowListDto(int id, string foto, string title, string isbn,
            string verlag, string autro, string studentName, DateTime borrowedUntil, int remainingDays)
        {
            Id = id;
            Foto = foto;
            Title = title;
            Isbn = isbn;
            Verlag = verlag;
            Autro = autro;
            StudentName = studentName;
            BorrowedUntil = borrowedUntil.ToString("d");
            RemainingDays = remainingDays;
        }
    }
}
