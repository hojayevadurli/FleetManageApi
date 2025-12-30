using FleetManage.Api.Data;
using FleetManage.Api.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static FleetManage.Api.DTOs.AuthDtos;

namespace FleetManage.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtTokenService _jwt;

        public AuthService(
            AppDbContext db,
            UserManager<AppUser> userManager,
            IJwtTokenService jwt)
        {
            _db = db;
            _userManager = userManager;
            _jwt = jwt;
        }

        public async Task<(bool ok, object? result, IEnumerable<string>? errors)>
            RegisterCompanyAsync(RegisterCompanyDto dto)
        {
            // 0) check duplicate email
            var existing = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existing != null)
                return (false, null, new[] { "Email already in use" });

            // 1) create tenant
            var tenant = new Tenant
            {
                Name = dto.CompanyName
            };

            await _db.Tenants.AddAsync(tenant);
            await _db.SaveChangesAsync();

            // 2) create user bound to that tenant
            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                TenantId = tenant.Id
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                // rollback tenant if user fails
                _db.Tenants.Remove(tenant);
                await _db.SaveChangesAsync();
                return (false, null, createResult.Errors.Select(e => e.Description));
            }

            // 3) issue JWT
            var token = _jwt.CreateToken(user);

            return (true, new
            {
                token,
                tenantId = tenant.Id,
                email = user.Email
            }, null);
        }
    }
}
