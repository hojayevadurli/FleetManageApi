using FleetManage.Api.Data;
using FleetManage.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetManage.Api.Controllers
{
    [ApiController]
    [Route("api/industries")]
    [AllowAnonymous] // Allow access during sign-up/creation if needed, otherwise [Authorize]
    public class IndustriesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public IndustriesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<IndustryDto>>> GetIndustries()
        {
            var list = await _db.Industries
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new IndustryDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}
