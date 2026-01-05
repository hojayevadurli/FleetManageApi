using System.Security.Claims;
using FleetManage.Api.Data;
using FleetManage.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetManage.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TenantsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TenantsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("current")]
        public async Task<ActionResult<TenantDto>> GetCurrentTenant()
        {
            var tenantIdString = User.FindFirstValue("tenantId");
            if (!Guid.TryParse(tenantIdString, out var tenantId))
            {
                return Unauthorized("Tenant ID not found in token.");
            }

            var tenant = await _db.Tenants
                .Include(t => t.Industry)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == tenantId);

            if (tenant == null)
            {
                return NotFound("Tenant not found.");
            }

            return Ok(new TenantDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                IndustryId = tenant.IndustryId,
                IndustryName = tenant.Industry?.Name,
                Email = tenant.Email,
                Phone = tenant.Phone,
                CreatedAt = tenant.CreatedAt
            });
        }

        [HttpPut("current")]
        public async Task<IActionResult> UpdateCurrentTenant([FromBody] UpdateTenantDto dto)
        {
            var tenantIdString = User.FindFirstValue("tenantId");
            if (!Guid.TryParse(tenantIdString, out var tenantId))
            {
                return Unauthorized("Tenant ID not found in token.");
            }

            var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
            if (tenant == null)
            {
                return NotFound("Tenant not found.");
            }

            tenant.Name = dto.Name;
            tenant.IndustryId = dto.IndustryId;
            tenant.Email = dto.Email;
            tenant.Phone = dto.Phone;
            
            // Optional: validate IndustryId exists if changed
            if (dto.IndustryId.HasValue && dto.IndustryId != tenant.IndustryId)
            {
                if (!await _db.Industries.AnyAsync(i => i.Id == dto.IndustryId))
                {
                    return BadRequest("Invalid Industry ID.");
                }
            }

            await _db.SaveChangesAsync();

            return NoContent();
        }

    }
}
