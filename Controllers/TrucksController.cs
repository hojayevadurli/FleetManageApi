using FleetManage.Api.Data;
using FleetManage.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetManage.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // only logged-in fleet manager
    public class TrucksController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TrucksController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/trucks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TruckDto>>> GetTrucks()
        {
            var trucks = await _db.Trucks
                .AsNoTracking()
                .Select(t => new TruckDto(
                    t.Id,
                    t.Number,
                    t.Vin,
                    t.Year,
                    t.Make,
                    t.Model,
                    t.PurchasedAt,
                    t.PlateNumber,
                    t.Mileage,
                    t.EngineType,
                    t.Status
                ))
                .ToListAsync();

            return Ok(trucks);
        }

        // GET: api/trucks/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TruckDto>> GetTruck(Guid id)
        {
            var t = await _db.Trucks
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new TruckDto(
                    x.Id,
                    x.Number,
                    x.Vin,
                    x.Year,
                    x.Make,
                    x.Model,
                    x.PurchasedAt,
                    x.PlateNumber,
                    x.Mileage,
                    x.EngineType,
                    x.Status
                ))
                .FirstOrDefaultAsync();

            if (t is null)
                return NotFound();

            return Ok(t);
        }

        // POST: api/trucks
        [HttpPost]
        public async Task<ActionResult<TruckDto>> CreateTruck([FromBody] CreateTruckDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Optional: enforce unique Number inside tenant
            var duplicate = await _db.Trucks.AnyAsync(t => t.Number == dto.Number);
            if (duplicate)
                return Conflict(new { message = "Truck number already exists." });

            var truck = new Truck
            {
                Number = dto.Number,
                Vin = dto.Vin,
                Year = dto.Year,
                Make = dto.Make,
                Model = dto.Model,
                PurchasedAt = dto.PurchasedAt,
                PlateNumber = dto.PlateNumber,
                Mileage = dto.Mileage,
                EngineType = dto.EngineType,
                Status = dto.Status
            };

            _db.Trucks.Add(truck);
            await _db.SaveChangesAsync();

            var result = new TruckDto(
                truck.Id,
                truck.Number,
                truck.Vin,
                truck.Year,
                truck.Make,
                truck.Model,
                truck.PurchasedAt,
                truck.PlateNumber,
                truck.Mileage,
                truck.EngineType,
                truck.Status
            );

            return CreatedAtAction(nameof(GetTruck), new { id = truck.Id }, result);
        }

        // PUT: api/trucks/{id}
        // PUT: api/trucks/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTruckDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Query filter already scopes by tenant
            var truck = await _db.Trucks.FirstOrDefaultAsync(t => t.Id == id);
            if (truck is null)
                return NotFound();

            // Optional: check if number is taken by another truck
            var numberTaken = await _db.Trucks
                .AnyAsync(t => t.Id != id && t.Number == dto.Number);

            if (numberTaken)
                return Conflict(new { message = "Truck number already exists." });

            // Apply updates with trimming/normalization
            truck.Number = dto.Number.Trim();
            truck.Vin = dto.Vin.Trim();
            truck.Year = dto.Year;
            truck.Make = string.IsNullOrWhiteSpace(dto.Make) ? null : dto.Make.Trim();
            truck.Model = string.IsNullOrWhiteSpace(dto.Model) ? null : dto.Model.Trim();
            truck.PurchasedAt = dto.PurchasedAt;
            truck.PlateNumber = string.IsNullOrWhiteSpace(dto.PlateNumber) ? null : dto.PlateNumber.Trim();
            truck.Mileage = dto.Mileage;
            truck.EngineType = string.IsNullOrWhiteSpace(dto.EngineType) ? null : dto.EngineType.Trim();
            truck.Status = string.IsNullOrWhiteSpace(dto.Status)
                ? null
                : dto.Status.Trim().ToLowerInvariant();

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // in case DB unique index on (TenantId, Number) fires anyway
                return BadRequest(new
                {
                    message = "Unable to update truck. Check if unit number is unique.",
                    detail = ex.Message
                });
            }

            return NoContent();
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Query filter ensures this is tenant-scoped
            var truck = await _db.Trucks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (truck == null)
                return NotFound();

            // If you want to block deleting trucks that have work orders, you can check here:
            // bool hasWorkOrders = await _db.WorkOrders.AnyAsync(w => w.TruckId == id);
            // if (hasWorkOrders) return BadRequest("Cannot delete truck with existing work orders.");

            _db.Trucks.Remove(truck);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        public sealed record BulkDeleteRequest(List<Guid> TruckIds);

        //api/trucks/bulk-delete
        public sealed record TrucksBulkDeleteRequest(List<Guid> TruckIds);

        //api/trucks/bulk-delete
        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] TrucksBulkDeleteRequest request)

        {
            if (request.TruckIds == null || request.TruckIds.Count == 0)
                return BadRequest(new { message = "No truck IDs provided." });

            // Tenant filter still applies
            var trucks = await _db.Trucks
                .Where(t => request.TruckIds.Contains(t.Id))
                .ToListAsync();

            if (trucks.Count == 0)
                return NotFound(new { message = "No trucks found for provided IDs." });

            // Same note as above – if you want to prevent deleting trucks with work orders,
            // check that here before RemoveRange.

            _db.Trucks.RemoveRange(trucks);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        
       

    }
}

