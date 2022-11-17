using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.DTOs
{
    public class BorrowOfStudentDto
    {
        public int Id { get; set; }
        public string Foto { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string Verlag { get; set; }
        public string Autro { get; set; }
        public string BorrowedUntil { get; set; }
        public int RemainingDays { get; set; }

        public BorrowOfStudentDto(int id, string foto, string title, string isbn,
            string verlag, string autro, DateTime borrowedUntil, int remainingDays)
        {
            Id = id;
            Foto = foto;
            Title = title;
            Isbn = isbn;
            Verlag = verlag;
            Autro = autro;
            BorrowedUntil = borrowedUntil.ToString("d");
            RemainingDays = remainingDays;
        }
    }
}
