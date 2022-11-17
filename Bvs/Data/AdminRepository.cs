using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bvs.Entities;
using Bvs_API.DTOs;
using Bvs_API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.Data
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public AdminRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AdminDto>> GetAdminsAsync()
        {
            return await _context.Admin
                .ProjectTo<AdminDto>(_mapper.ConfigurationProvider).ToListAsync();

            //return await _context.Admin
            //    .Select(x => new AdminDto(x.Id, x.Name, x.Vorname, x.Email, x.Foto, x.Rolle)).ToListAsync();
        }

        public async Task<Admin> GetAdminByIdAsync(int id)
        {
            return await _context.Admin.FindAsync(id);
        }

        public async Task<Admin> GetAdminByUsernameAsync(string username)
        {
            return await _context.Admin.SingleOrDefaultAsync(x => x.Username.Equals(username));
        }

        public async Task<Admin> GetAdminByEmailAsync(string email)
        {
            return await _context.Admin.SingleOrDefaultAsync(x => x.Email.Equals(email));
        }

        public async Task<bool> IsAdminUsernameExistsAsync(string username)
        {
            return await _context.Admin.AnyAsync(x => string.Equals(x.Username, username));
        }

        public async Task<bool> IsAdminEmailExistsAsync(string email)
        {
            return await _context.Admin.AnyAsync(x => x.Email.Equals(email));
        }

        public void AddAdmin(Admin admin)
        {
            _context.Admin.Add(admin);
        }

        public void RemoveAdmin(Admin admin)
        {
            _context.Admin.Remove(admin);
        }
       
    }
}
