using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.DTOs
{
    public class AdminDto
    {
        public AdminDto(int id, string name, string vorname, string email, string foto, string rolle)
        {
            Id = id;
            Name = name;
            Vorname = vorname;
            Email = email;
            Foto = foto;
            Rolle = rolle;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Vorname { get; set; }

        public string Email { get; set; }

        public string Foto { get; set; }

        public string Rolle { get; set; }
    }
}
