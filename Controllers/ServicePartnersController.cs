using FleetManage.Api.Data;
using FleetManage.Api.Data.Enums;
using FleetManage.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FleetManage.Api.DTOs.ServicePartnerDtos;

namespace FleetManage.Api.Controllers
{
    [ApiController]
    [Route("api/service-partners")]
    [Authorize]
    public class ServicePartnersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ServicePartnersController(AppDbContext db)
        {
            _db = db;
        }

        // ----------------------------
        // GET: api/service-partners
        // Filters: search, tier, minRating, pricing, city, state, industryId
        // ----------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicePartnerDto>>> GetServicePartners(
            [FromQuery] string? search,
            [FromQuery] string? tier,
            [FromQuery] int? minRating,
            [FromQuery] string? pricing,
            [FromQuery] string? city,
            [FromQuery] string? state,
            [FromQuery] int? industryId,
            [FromQuery] string? category // support UI 'category' param mapping to Tier
        )
        {
            var query = _db.ServicePartners.AsNoTracking();

            // Filter: Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(x => 
                    x.Name.ToLower().Contains(s) || 
                    x.Address1.ToLower().Contains(s) ||
                    x.City.ToLower().Contains(s) ||
                    (x.ContactName != null && x.ContactName.ToLower().Contains(s))
                );
            }

            // Filter: Industry
            if (industryId.HasValue)
            {
                query = query.Where(x => x.IndustryId == industryId.Value);
            }

            // Filter: Location
            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(x => x.City.ToLower() == city.ToLower());
            
            if (!string.IsNullOrWhiteSpace(state))
                query = query.Where(x => x.State.ToLower() == state.ToLower());


            // Filter: Tier (Category)
            // UI sends "green", "orange", "red" or "Preferred", "Standard" etc.
            string? tierFilter = tier;
            if (string.IsNullOrWhiteSpace(tierFilter) && !string.IsNullOrWhiteSpace(category))
            {
                tierFilter = category.ToLower() switch
                {
                    "green" => "Preferred",
                    "orange" => "Standard",
                    "red" => "Warning",
                    "all" => null,
                    _ => null
                };
            }

            if (!string.IsNullOrWhiteSpace(tierFilter) && tierFilter != "all")
            {
                if (Enum.TryParse<ServicePartnerTier>(tierFilter, true, out var tEnum))
                {
                    query = query.Where(x => x.NetworkTier == tEnum);
                }
            }

            // Filter: Min Rating
            if (minRating.HasValue && minRating > 0)
            {
                query = query.Where(x => x.AverageRating >= minRating.Value);
            }

            // Filter: Pricing
            if (!string.IsNullOrWhiteSpace(pricing) && pricing != "all")
            {
                query = query.Where(x => x.PricingStrategy == pricing);
            }

            var list = await query.ToListAsync();

            return Ok(list.Select(MapToDto));
        }

        // ----------------------------
        // GET: api/service-partners/{id}
        // ----------------------------
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ServicePartnerDto>> GetServicePartner(Guid id)
        {
            var shop = await _db.ServicePartners.FindAsync(id);
            if (shop == null) return NotFound();

            return Ok(MapToDto(shop));
        }

        // ----------------------------
        // POST: api/service-partners
        // ----------------------------
        [HttpPost]
        public async Task<ActionResult<ServicePartnerDto>> CreateServicePartner([FromBody] CreateServicePartnerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Parse Enums
            ServicePartnerType typeEnum = ServicePartnerType.Shop;
            if (!string.IsNullOrWhiteSpace(dto.Type) && Enum.TryParse(dto.Type, true, out ServicePartnerType t))
                typeEnum = t;

            ServicePartnerTier tierEnum = ServicePartnerTier.Standard;
            if (!string.IsNullOrWhiteSpace(dto.NetworkTier))
            {
                // Unify colors/names
                var raw = dto.NetworkTier.ToLower();
                if (raw == "green") tierEnum = ServicePartnerTier.Preferred;
                else if (raw == "red") tierEnum = ServicePartnerTier.Warning;
                else if (raw == "orange") tierEnum = ServicePartnerTier.Standard;
                else Enum.TryParse(dto.NetworkTier, true, out tierEnum);
            }

            var shop = new ServicePartner
            {
                Name = dto.Name,
                Address1 = dto.Address1,
                Address2 = dto.Address2,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country ?? "USA",
                
                Phone = dto.Phone,
                Email = dto.Email,
                Website = dto.Website,
                ContactName = dto.ContactName,
                
                IndustryId = dto.IndustryId,
                Type = typeEnum,

                LaborRate = dto.LaborRate,
                Specialties = dto.Specialties ?? new List<string>(),
                NetworkTier = tierEnum,
                PricingStrategy = dto.PricingStrategy ?? "$$",
                Notes = dto.Notes
            };

            _db.ServicePartners.Add(shop);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServicePartner), new { id = shop.Id }, MapToDto(shop));
        }

        // ----------------------------
        // PUT: api/service-partners/{id}
        // ----------------------------
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateServicePartner(Guid id, [FromBody] UpdateServicePartnerDto dto)
        {
            var shop = await _db.ServicePartners.FindAsync(id);
            if (shop == null) return NotFound();

            shop.Name = dto.Name;
            
            shop.Address1 = dto.Address1;
            shop.Address2 = dto.Address2;
            shop.City = dto.City;
            shop.State = dto.State;
            shop.PostalCode = dto.PostalCode;
            shop.Country = dto.Country ?? "USA";

            shop.Phone = dto.Phone;
            shop.Email = dto.Email;
            shop.Website = dto.Website;
            shop.ContactName = dto.ContactName;
            
            if (dto.IndustryId.HasValue) shop.IndustryId = dto.IndustryId;

            if (!string.IsNullOrWhiteSpace(dto.Type) && Enum.TryParse<ServicePartnerType>(dto.Type, true, out var t))
                shop.Type = t;

            shop.LaborRate = dto.LaborRate;
            
            // Tier
            if (!string.IsNullOrWhiteSpace(dto.NetworkTier))
            {
                var raw = dto.NetworkTier.ToLower();
                if (raw == "green") shop.NetworkTier = ServicePartnerTier.Preferred;
                else if (raw == "red") shop.NetworkTier = ServicePartnerTier.Warning;
                else if (raw == "orange") shop.NetworkTier = ServicePartnerTier.Standard;
                else if (Enum.TryParse<ServicePartnerTier>(dto.NetworkTier, true, out var tier)) shop.NetworkTier = tier;
            }

            shop.PricingStrategy = dto.PricingStrategy ?? shop.PricingStrategy;
            shop.IsActive = dto.IsActive;
            shop.Notes = dto.Notes;

            if (dto.Specialties != null)
            {
                shop.Specialties = dto.Specialties;
            }

            shop.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // ----------------------------
        // DELETE: api/service-partners/{id}
        // ----------------------------
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteServicePartner(Guid id)
        {
            var shop = await _db.ServicePartners.FindAsync(id);
            if (shop == null) return NotFound();

            _db.ServicePartners.Remove(shop);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // ----------------------------
        // RATINGS & HISTORY (Unchanged logic, just mapping fix if needed)
        // ----------------------------

        [HttpGet("{id:guid}/ratings")]
        public async Task<ActionResult<IEnumerable<ServicePartnerRatingDto>>> GetRatings(Guid id)
        {
            var exists = await _db.ServicePartners.AnyAsync(x => x.Id == id);
            if (!exists) return NotFound("Shop not found");

            var ratings = await _db.ServicePartnerRatings
                .Where(x => x.ServicePartnerId == id)
                .OrderByDescending(x => x.CreatedAt)
                .Select(r => new ServicePartnerRatingDto(
                    r.Id, 
                    r.ServicePartnerId, 
                    r.Rating, 
                    r.ReviewText, 
                    r.CreatedAt))
                .ToListAsync();

            return Ok(ratings);
        }

        [HttpPost("{id:guid}/ratings")]
        public async Task<IActionResult> AddRating(Guid id, [FromBody] CreateRatingDto dto)
        {
            var shop = await _db.ServicePartners.Include(x => x.Ratings).FirstOrDefaultAsync(x => x.Id == id);
            if (shop == null) return NotFound("Shop not found");

            var rating = new ServicePartnerRating
            {
                ServicePartnerId = id,
                Rating = dto.Rating,
                ReviewText = dto.ReviewText
            };

            _db.ServicePartnerRatings.Add(rating);
            await _db.SaveChangesAsync();

            var stats = await _db.ServicePartnerRatings
                .Where(x => x.ServicePartnerId == id)
                .GroupBy(x => x.ServicePartnerId)
                .Select(g => new { 
                    Avg = g.Average(r => r.Rating), 
                    Count = g.Count() 
                })
                .FirstOrDefaultAsync();

            if (stats != null)
            {
                shop.AverageRating = (decimal)stats.Avg;
                shop.ReviewCount = stats.Count;
                await _db.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet("{id:guid}/history")]
        public async Task<ActionResult<IEnumerable<object>>> GetHistory(Guid id)
        {
            var history = await _db.WorkOrders
                .AsNoTracking()
                .Where(x => x.VendorId == id && !x.IsDeleted)
                .OrderByDescending(x => x.InvoiceDate ?? x.OpenedAt.DateTime)
                .Select(x => new 
                {
                    x.Id,
                    InvoiceDate = x.InvoiceDate ?? x.OpenedAt.DateTime,
                    InvoiceNumber = x.InvoiceNumber ?? x.WorkOrderNumber,
                    EquipmentId = x.EquipmentId,
                    Summary = x.Title,
                    TotalAmount = x.ManualActualTotal ?? x.EstimatedTotal ?? 0,
                    Status = x.Status.ToString()
                })
                .ToListAsync();

            return Ok(history);
        }

        // ----------------------------
        // MAPPING
        // ----------------------------
        private static ServicePartnerDto MapToDto(ServicePartner s)
        {
            return new ServicePartnerDto(
                s.Id,
                s.Name,
                s.Address1,
                s.Address2,
                s.City,
                s.State,
                s.PostalCode,
                s.Country,

                s.Phone,
                s.Email,
                s.Website,
                s.ContactName,
                
                s.IndustryId,
                s.Type.ToString(),

                s.LaborRate,
                s.Specialties,
                s.Latitude,
                s.Longitude,
                s.NetworkTier.ToString(), // "Preferred" etc.
                s.PricingStrategy,
                s.AuditScore,
                s.AverageRating,
                s.ReviewCount,
                s.IsActive,
                s.Notes,
                s.CreatedAt
            );
        }
    }
}
