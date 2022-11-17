using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bvs.Entities;
using Bvs_API.Data;
using Bvs_API.DTOs;
using Bvs_API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bvs_API.Controllers
{
    
    public class AdminController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IAdminRepository _adminRepo;

        public AdminController(DataContext context, ITokenService tokenService, IMapper mapper, IAdminRepository adminRepository)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
            _adminRepo = adminRepository;
        }

        [HttpGet("Sync")]
        public Admin GetAdminsTestEquals(string username)
        {
            return _context.Admin.SingleOrDefault(x => x.Username.ToLower() == username.ToLower());
        }

        [HttpGet("Async")]
        public async Task<Admin> GetAdminsTestTolower(string username)
        {
            return await _context.Admin.SingleOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());
        }



        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminDto>>> GetAdmins()
        {
            return Ok(await _adminRepo.GetAdminsAsync());
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AdminDto>> GetAdmin(int id)
        {
            Admin admin = await _adminRepo.GetAdminByIdAsync(id);

            if (admin == null)
            {
                return NotFound($"Admin mit Id {id} ist nicht existiert.");
            }

            AdminDto adminDto = _mapper.Map<AdminDto>(admin);
            return Ok(adminDto);
        }

        [Authorize]
        [HttpPost("addAdmin")]
        public async Task<ActionResult<AdminDto>> AddAdmin(AdminRegisterDto adminDto)
        {
            if (await _adminRepo.IsAdminUsernameExistsAsync(adminDto.Username))
            {
                return BadRequest("Admin username is taken.");
            }

            if (await _adminRepo.IsAdminEmailExistsAsync(adminDto.Email))
            {
                return BadRequest("Admin Email is taken.");
            }

            if(adminDto.Password != adminDto.ConfirmPassword)
            {
                return BadRequest("Passwörte stimmen nicht überein!");
            }

            Admin admin = _mapper.Map<Admin>(adminDto);
            admin.Foto = "";

            using var hmac = new HMACSHA512();
            admin.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(adminDto.Password));
            admin.PasswordSalt = hmac.Key;

            _adminRepo.AddAdmin(admin);
            await _context.SaveChangesAsync();

            await AdminsCount();
            await _context.SaveChangesAsync();

            AdminDto registeredAdmin = _mapper.Map<AdminDto>(admin);

            return registeredAdmin;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AdminTokenDto>> LoginAdmin(AdminLoginDto adminLoginDto)
        {
            Admin admin = await _adminRepo.GetAdminByUsernameAsync(adminLoginDto.UsernameOrEmail);

            if (admin == null)
            {
                admin = await _adminRepo.GetAdminByEmailAsync(adminLoginDto.UsernameOrEmail);
            }

            if (admin == null)
            {
                return Unauthorized("Username or email is invalid.");
            }

            bool password = CheckPassword(admin.PasswordSalt, admin.PasswordHash, adminLoginDto.Password);

            if (!password)
            {
                return Unauthorized("Password is invalid.");
            }

            AdminTokenDto adminToken = _mapper.Map<AdminTokenDto>(admin);
            adminToken.Token = _tokenService.CreateToken(admin);

            return adminToken;
        }

        [Authorize]
        [HttpPut("editAdmin")]
        public async Task<ActionResult<AdminTokenDto>> EditProfil(AdminEditDto adminEdit)
        {
            Admin admin = await _adminRepo.GetAdminByEmailAsync(adminEdit.Email);

            if (admin == null)
            {
                return NotFound($"Admin mit Emailadresse: {adminEdit.Email} wurde nicht gefunden.");
            }
            
            bool password = CheckPassword(admin.PasswordSalt, admin.PasswordHash, adminEdit.Password);

            if (!password)
            {
                return Unauthorized("Password is invalid.");
            }

            if (await _adminRepo.IsAdminUsernameExistsAsync(adminEdit.Username) 
                && admin.Username != adminEdit.Username)
            {
                return BadRequest("Admin username is taken.");
            }

            _mapper.Map(adminEdit, admin);
            admin.Foto = "";

            await _context.SaveChangesAsync();

            AdminTokenDto adminToken = _mapper.Map<AdminTokenDto>(admin);
            adminToken.Token = _tokenService.CreateToken(admin);

            return adminToken;
        }

        [Authorize]
        [HttpPut("changePassword")]
        public async Task<ActionResult<AdminTokenDto>> ChangePassword(AdminPasswordDto adminPassword)
        {
            Admin admin = await _adminRepo.GetAdminByEmailAsync(adminPassword.Email);

            if (admin == null)
            {
                return NotFound($"Admin mit Emailadresse: {adminPassword.Email} wurde nicht gefunden.");
            }

            // check the old password

            bool password = CheckPassword(admin.PasswordSalt, admin.PasswordHash, adminPassword.Password);

            if (!password)
            {
                return Unauthorized("Password is invalid.");
            }

            // check if the password right confirmed

            if(adminPassword.NewPassword != adminPassword.ConfirmPassword)
            {
                return Unauthorized("The passwords do not match");
            }

            // change password to new 

            using var hmac = new HMACSHA512();

            admin.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(adminPassword.NewPassword));
            admin.PasswordSalt = hmac.Key;

            await _context.SaveChangesAsync();

            AdminTokenDto adminToken = _mapper.Map<AdminTokenDto>(admin);
            adminToken.Token = _tokenService.CreateToken(admin);

            return adminToken;
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAdmin(int id)
        {
            var admin = await _adminRepo.GetAdminByIdAsync(id);

            if (admin == null)
            {
                return NotFound($"Admin with Id= {id} not found");
            }
            
            _adminRepo.RemoveAdmin(admin);
            await _context.SaveChangesAsync();

            await AdminsCount();
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Check Password

        private bool CheckPassword(byte[] PasswordSaltFromDB, 
            byte[] PasswordHashFromDB, string PasswordFromClient)
        {
            using var hmac = new HMACSHA512(PasswordSaltFromDB);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(PasswordFromClient));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != PasswordHashFromDB[i])
                {
                    return false;
                }
            }

            return true;
        }

        // Admin counter for overview Page

        private async Task AdminsCount()
        {
            var adminCount = await _context.Admin.CountAsync();
            var numberOverview = await _context.NumberOverview.FirstAsync();
            numberOverview.AnzahlAdmin = adminCount;
        }
    }
}
