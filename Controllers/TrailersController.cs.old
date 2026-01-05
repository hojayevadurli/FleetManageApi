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
    public class TrailersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TrailersController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/trailers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrailerDto>>> GetTrailers()
        {
            // Tenant filter comes from DbContext global query filter
            var trailers = await _db.Trailers
                .AsNoTracking()
                .Select(tr => new TrailerDto(
                    tr.Id,
                    tr.Number,
                    tr.Vin,
                    tr.Year,
                    tr.Make,
                    tr.Model,
                    tr.PurchasedAt,
                    tr.Type,
                    tr.Length,
                    tr.WeightCapacity,
                    tr.Status
                ))
                .ToListAsync();

            return Ok(trailers);
        }

        // GET: api/trailers/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TrailerDto>> GetTrailer(Guid id)
        {
            var tr = await _db.Trailers
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new TrailerDto(
                    x.Id,
                    x.Number,
                    x.Vin,
                    x.Year,
                    x.Make,
                    x.Model,
                    x.PurchasedAt,
                    x.Type,
                    x.Length,
                    x.WeightCapacity,
                    x.Status
                ))
                .FirstOrDefaultAsync();

            if (tr is null)
                return NotFound();

            return Ok(tr);
        }

        // POST: api/trailers
        [HttpPost]
        public async Task<ActionResult<TrailerDto>> CreateTrailer([FromBody] CreateTrailerDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Scoped per-tenant thanks to query filter + unique index on (TenantId, Number)
            var duplicate = await _db.Trailers.AnyAsync(t => t.Number == dto.Number);
            if (duplicate)
                return Conflict(new { message = "Trailer number already exists." });

            var trailer = new Trailer
            {
                // TenantId will be set automatically by InjectTenantIds()
                Number = dto.Number.Trim(),
                Vin = dto.Vin.Trim(),
                Year = dto.Year,
                Make = string.IsNullOrWhiteSpace(dto.Make) ? null : dto.Make.Trim(),
                Model = string.IsNullOrWhiteSpace(dto.Model) ? null : dto.Model.Trim(),
                PurchasedAt = dto.PurchasedAt,
                Type = string.IsNullOrWhiteSpace(dto.Type) ? null : dto.Type.Trim(),
                Length = dto.Length,
                WeightCapacity = dto.WeightCapacity,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? null : dto.Status.Trim()
            };

            _db.Trailers.Add(trailer);
            await _db.SaveChangesAsync();

            var result = new TrailerDto(
                trailer.Id,
                trailer.Number,
                trailer.Vin,
                trailer.Year,
                trailer.Make,
                trailer.Model,
                trailer.PurchasedAt,
                trailer.Type,
                trailer.Length,
                trailer.WeightCapacity,
                trailer.Status
            );

            return CreatedAtAction(nameof(GetTrailer), new { id = trailer.Id }, result);
        }

        // PUT: api/trailers/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTrailer(Guid id, [FromBody] UpdateTrailerDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Global query filter already scopes by tenant
            var trailer = await _db.Trailers.FirstOrDefaultAsync(t => t.Id == id);
            if (trailer is null)
                return NotFound();

            // Unique number within same tenant (filter already applied)
            var numberTaken = await _db.Trailers
                .AnyAsync(t => t.Id != id && t.Number == dto.Number);

            if (numberTaken)
                return Conflict(new { message = "Trailer number already exists." });

            trailer.Number = dto.Number.Trim();
            trailer.Vin = dto.Vin.Trim();
            trailer.Year = dto.Year;
            trailer.Make = string.IsNullOrWhiteSpace(dto.Make) ? null : dto.Make.Trim();
            trailer.Model = string.IsNullOrWhiteSpace(dto.Model) ? null : dto.Model.Trim();
            trailer.PurchasedAt = dto.PurchasedAt;
            trailer.Type = string.IsNullOrWhiteSpace(dto.Type) ? null : dto.Type.Trim();
            trailer.Length = dto.Length;
            trailer.WeightCapacity = dto.WeightCapacity;
            trailer.Status = string.IsNullOrWhiteSpace(dto.Status)
                ? null
                : dto.Status.Trim().ToLowerInvariant();

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/trailers/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTrailer(Guid id)
        {
            // Tenant filter applied automatically
            var trailer = await _db.Trailers.FirstOrDefaultAsync(t => t.Id == id);
            if (trailer is null)
                return NotFound();

            _db.Trailers.Remove(trailer);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // Inside namespace FleetManage.Api.Controllers, in TrailersController

        public sealed record BulkDeleteTrailersRequest(List<Guid> TrailerIds);

        // POST: api/trailers/bulk-delete
        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteTrailersRequest request)
        {
            if (request.TrailerIds == null || request.TrailerIds.Count == 0)
                return BadRequest(new { message = "No trailer IDs provided." });

            var trailers = await _db.Trailers
                .Where(t => request.TrailerIds.Contains(t.Id))
                .ToListAsync();

            if (trailers.Count == 0)
                return NotFound(new { message = "No trailers found for provided IDs." });

            _db.Trailers.RemoveRange(trailers);
            await _db.SaveChangesAsync();

            return NoContent();
        }

    }
}
