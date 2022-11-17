using Bvs.Entities;
using Bvs_API.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.Interfaces
{
  
    public interface IAdminRepository
    {
        Task<IEnumerable<AdminDto>> GetAdminsAsync();

        Task<Admin> GetAdminByIdAsync(int id);

        Task<Admin> GetAdminByUsernameAsync(string name);

        Task<Admin> GetAdminByEmailAsync(string email);

        Task<bool> IsAdminUsernameExistsAsync(string username);

        Task<bool> IsAdminEmailExistsAsync(string email);

        void AddAdmin(Admin admin);

        void RemoveAdmin(Admin admin);
    }
}
