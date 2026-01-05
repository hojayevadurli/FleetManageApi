using FleetManage.Api.Data;
using FleetManage.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetManage.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FleetCategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FleetCategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/FleetCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FleetCategoryDto>>> GetFleetCategories([FromQuery] int? industryId)
        {
            var query = _context.FleetCategories
                .Include(d => d.Industry)
                .AsQueryable();

            if (industryId.HasValue)
            {
                query = query.Where(d => d.IndustryId == industryId.Value);
            }

            return await query
                .Select(d => new FleetCategoryDto
                {
                    Id = d.Id,
                    IndustryId = d.IndustryId,
                    IndustryName = d.Industry.Name,
                    Name = d.Name,
                    Code = d.Code,
                    IsActive = d.IsActive,
                    ListEquipment = d.ListEquipment
                })
                .ToListAsync();
        }

        // GET: api/FleetCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FleetCategoryDto>> GetFleetCategory(int id)
        {
            var fleetCategory = await _context.FleetCategories
                .Include(d => d.Industry)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (fleetCategory == null)
            {
                return NotFound();
            }

            return new FleetCategoryDto
            {
                Id = fleetCategory.Id,
                IndustryId = fleetCategory.IndustryId,
                IndustryName = fleetCategory.Industry.Name,
                Name = fleetCategory.Name,
                Code = fleetCategory.Code,
                IsActive = fleetCategory.IsActive,
                ListEquipment = fleetCategory.ListEquipment
            };
        }

        // POST: api/FleetCategories
        [HttpPost]
        public async Task<ActionResult<FleetCategoryDto>> PostFleetCategory(CreateFleetCategoryDto dto)
        {
            if (await _context.FleetCategories.AnyAsync(e => e.Id == dto.Id))
            {
                return Conflict($"FleetCategory with ID {dto.Id} already exists.");
            }

            var fleetCategory = new FleetCategory
            {
                Id = dto.Id,
                IndustryId = dto.IndustryId,
                Name = dto.Name,
                Code = dto.Code,
                IsActive = dto.IsActive,
                ListEquipment = dto.ListEquipment
            };

            _context.FleetCategories.Add(fleetCategory);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FleetCategoryExists(fleetCategory.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // Reload to get Industry info
            await _context.Entry(fleetCategory).Reference(d => d.Industry).LoadAsync();

            var resultDto = new FleetCategoryDto
            {
                Id = fleetCategory.Id,
                IndustryId = fleetCategory.IndustryId,
                IndustryName = fleetCategory.Industry.Name,
                Name = fleetCategory.Name,
                Code = fleetCategory.Code,
                IsActive = fleetCategory.IsActive,
                ListEquipment = fleetCategory.ListEquipment
            };

            return CreatedAtAction(nameof(GetFleetCategory), new { id = fleetCategory.Id }, resultDto);
        }

        // PUT: api/FleetCategories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFleetCategory(int id, UpdateFleetCategoryDto dto)
        {
            var fleetCategory = await _context.FleetCategories.FindAsync(id);

            if (fleetCategory == null)
            {
                return NotFound();
            }

            fleetCategory.IndustryId = dto.IndustryId;
            fleetCategory.Name = dto.Name;
            fleetCategory.Code = dto.Code;
            fleetCategory.IsActive = dto.IsActive;
            fleetCategory.ListEquipment = dto.ListEquipment;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FleetCategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/FleetCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFleetCategory(int id)
        {
            var fleetCategory = await _context.FleetCategories.FindAsync(id);
            if (fleetCategory == null)
            {
                return NotFound();
            }

            _context.FleetCategories.Remove(fleetCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FleetCategoryExists(int id)
        {
            return _context.FleetCategories.Any(e => e.Id == id);
        }
    }
}
