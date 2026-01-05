using FleetManage.Api.Data;
using FleetManage.Api.DTOs;
using FleetManage.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FleetManage.Api.Data.Enums;

namespace FleetManage.Api.Controllers
{
    [ApiController]
    [Route("api/service-history")]
    [Authorize]
    public class ServiceHistoryController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITenantContext _tenant;

        public ServiceHistoryController(AppDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Guid? equipmentId)
        {
            var q = _db.ServiceHistories
                .AsNoTracking()
                .Where(x => x.TenantId == _tenant.TenantId && x.DeletedStatus != 1);

            if (equipmentId.HasValue)
            {
                q = q.Where(x => x.EquipmentId == equipmentId.Value);
            }

            var data = await q
                .Include(x => x.Lines)
                .OrderByDescending(x => x.InvoiceDate ?? x.CreatedAt)
                .ToListAsync();

            return Ok(data);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceHistoryUpsertDto dto)
        {
            var entity = new ServiceHistory
            {
                TenantId = _tenant.TenantId,
                EquipmentId = dto.EquipmentId,
                WorkOrderId = dto.WorkOrderId,
                VendorNameRaw = dto.VendorNameRaw,
                InvoiceNumber = dto.InvoiceNumber,
                InvoiceDate = dto.InvoiceDate,
                Odometer = dto.Odometer,
                TotalAmount = dto.TotalAmount,
                TaxAmount = dto.TaxAmount,
                Summary = dto.Summary,
                Category = dto.Category,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                //ModifiedOn = DateTime.UtcNow
            };
            
                if (dto.WorkOrderId.HasValue)
                {
                    // VERIFY WorkOrder Exists to prevent FK violation
                    var woExists = await _db.WorkOrders.AnyAsync(w => w.Id == dto.WorkOrderId.Value);
                    if (!woExists)
                    {
                        entity.WorkOrderId = null; // Unlink if invalid ID passed
                    }
                }

                // Auto-Link or Auto-Create Shop Logic
                if (dto.VendorId.HasValue)
                {
                    entity.VendorId = dto.VendorId;
                }
                else if (!string.IsNullOrWhiteSpace(dto.VendorNameRaw))
                {
                    // Try to find existing shop by name (case-insensitive)
                    var rawName = dto.VendorNameRaw.Trim();
                    var existingShop = await _db.ServicePartners
                        .FirstOrDefaultAsync(s => s.Name.ToLower() == rawName.ToLower());

                    if (existingShop != null)
                    {
                        entity.VendorId = existingShop.Id;
                    }
                    else
                    {
                        // AUTO-CREATE NEW SHOP
                        try 
                        {
                            var newShop = new ServicePartner
                            {
                                TenantId = _tenant.TenantId,
                                Name = rawName,
                                Address1 = "Unknown Address - Auto Created",
                                City = "Unknown",
                                State = "Unknown",
                                PostalCode = "00000",
                                Country = "USA",
                                Phone = "",
                                Email = "",
                                Website = "",
                                ContactName = "",
                                NetworkTier = ServicePartnerTier.Standard,
                                PricingStrategy = "$$",
                                IsActive = true,
                                Notes = "Auto-created from uploaded invoice",
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };
                            _db.ServicePartners.Add(newShop);
                            await _db.SaveChangesAsync();

                            entity.VendorId = newShop.Id;
                        }
                        catch (Exception)
                        {
                            // If shop creation fails (constraint, etc), ignore and just save History with raw name
                            // This prevents 500 error for the whole request
                            entity.VendorId = null;
                        }
                    }
                }

            foreach (var li in dto.Lines)
            {
                entity.Lines.Add(new ServiceHistoryLine
                {
                    TenantId = _tenant.TenantId,
                    Type = li.Type,
                    Description = li.Description,
                    Qty = li.Qty,
                    UnitPrice = li.UnitPrice,
                    Amount = li.Amount,
                    PartNumber = li.PartNumber
                });
            }

            _db.ServiceHistories.Add(entity);
            await _db.SaveChangesAsync();

            return Ok(entity.Id);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ServiceHistoryUpsertDto dto)
        {
            var entity = await _db.ServiceHistories
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null) return NotFound();

            entity.EquipmentId = dto.EquipmentId;
            entity.WorkOrderId = dto.WorkOrderId;
            entity.VendorId = dto.VendorId;
            entity.VendorNameRaw = dto.VendorNameRaw;
            entity.InvoiceNumber = dto.InvoiceNumber;
            entity.InvoiceDate = dto.InvoiceDate;
            entity.Odometer = dto.Odometer;
            entity.TotalAmount = dto.TotalAmount;
            entity.TaxAmount = dto.TaxAmount;
            entity.Summary = dto.Summary;
            entity.Category = dto.Category;
            entity.Status = dto.Status;
           // entity.ModifiedOn = DateTime.UtcNow;

            // replace line items (same pattern as WorkOrder)
            _db.ServiceHistoryLines.RemoveRange(entity.Lines);
            entity.Lines.Clear();

            foreach (var li in dto.Lines)
            {
                entity.Lines.Add(new ServiceHistoryLine
                {
                    TenantId = entity.TenantId,
                    Type = li.Type,
                    Description = li.Description,
                    Qty = li.Qty,
                    UnitPrice = li.UnitPrice,
                    Amount = li.Amount,
                    PartNumber = li.PartNumber
                });
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var entity = await _db.ServiceHistories.FirstOrDefaultAsync(x => x.Id == id);
            if (entity is null) return NotFound();

            if (entity.DeletedStatus == 1)
                return NoContent();

            entity.DeletedStatus = 1;
            entity.DeletedAt = DateTime.UtcNow;
            //entity.ModifiedOn = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
