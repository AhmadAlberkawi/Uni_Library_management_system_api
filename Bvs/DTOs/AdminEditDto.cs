using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.DTOs
{
    public class AdminEditDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Vorname { get; set; }
        [Required]
        public string Email { get; set; }
        
        public string Foto { get; set; }
        [Required]
        public string Rolle { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
