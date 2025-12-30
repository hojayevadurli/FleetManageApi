using FleetManage.Api.Data;
using FleetManage.Api.DTOs;
using FleetManage.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> List(
     [FromQuery] string? assetType,
     [FromQuery] Guid? assetId)
        {
            var q = _db.ServiceHistories
                .AsNoTracking()
                .Where(x => x.TenantId == _tenant.TenantId && x.DeletedStatus != 1);

            // apply filters only if provided
            if (!string.IsNullOrWhiteSpace(assetType))
            {
                assetType = assetType.Trim().ToLowerInvariant();
                if (assetType is not ("truck" or "trailer"))
                    return BadRequest("assetType must be 'truck' or 'trailer'.");

                q = q.Where(x => x.AssetType == assetType);
            }

            if (assetId.HasValue)
            {
                q = q.Where(x => x.AssetId == assetId.Value);
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
                AssetType = dto.AssetType,
                AssetId = dto.AssetId,
                WorkOrderId = dto.WorkOrderId,
                VendorId = dto.VendorId,
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

            entity.AssetType = dto.AssetType;
            entity.AssetId = dto.AssetId;
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
