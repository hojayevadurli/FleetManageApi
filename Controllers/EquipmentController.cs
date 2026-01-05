using FleetManage.Api.Data;
using FleetManage.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetManage.Api.Controllers
{
    [ApiController]
    [Route("api/equipment")]
    [Authorize]
    public class EquipmentController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly FleetManage.Api.Services.INhtsaRecallService _recalls;

        public EquipmentController(AppDbContext db, FleetManage.Api.Services.INhtsaRecallService recalls)
        {
            _db = db;
            _recalls = recalls;
        }

        [HttpGet]
        public async Task<ActionResult<List<EquipmentDto>>> GetAll([FromQuery] int? typeId, [FromQuery] int? fleetCategoryId)
        {
            var query = _db.Equipments
                .Include(e => e.EquipmentType)
                .Include(e => e.FleetCategory)
                .Include(e => e.Recalls)
                    .ThenInclude(r => r.RecallCampaign)
                .AsNoTracking();

            if (typeId.HasValue) query = query.Where(x => x.EquipmentTypeId == typeId);
            if (fleetCategoryId.HasValue) query = query.Where(x => x.FleetCategoryId == fleetCategoryId);

            var list = await query
                .OrderBy(x => x.UnitNumber)
                .Select(x => new EquipmentDto
                {
                    Id = x.Id,
                    EquipmentTypeId = x.EquipmentTypeId,
                    EquipmentTypeName = x.EquipmentType.Name,
                    EquipmentTypeCode = x.EquipmentType.Code ?? "",
                    FleetCategoryId = x.FleetCategoryId,
                    FleetCategoryName = x.FleetCategory.Name,
                    UnitNumber = x.UnitNumber,
                    DisplayName = x.DisplayName,
                    Vin = x.Vin,
                    SerialNumber = x.SerialNumber,
                    PlateNumber = x.PlateNumber,
                    Make = x.Make,
                    Model = x.Model,
                    Year = x.Year,
                    LifecycleStatus = x.LifecycleStatus,
                    OperationalStatus = x.OperationalStatus,
                    OdometerCurrent = x.OdometerCurrent,
                    HoursCurrent = x.HoursCurrent,
                    AcquiredDate = x.AcquiredDate,
                    InServiceDate = x.InServiceDate,
                    OutOfServiceDate = x.OutOfServiceDate,
                    Notes = x.Notes,
                    Recalls = x.Recalls.Select(r => new EquipmentRecallDto
                    {
                        Id = r.Id,
                        CampaignCode = r.RecallCampaign.Code,
                        CampaignTitle = r.RecallCampaign.Title,
                        CampaignDescription = r.RecallCampaign.Description,
                        Manufacturer = r.RecallCampaign.Manufacturer,
                        IssueDate = r.RecallCampaign.IssueDate,
                        Status = r.Status,
                        FirstSeenAt = r.FirstSeenAt,
                        ResolvedAt = r.ResolvedAt,
                        Notes = r.Notes
                    }).ToList()
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpPost("{id:guid}/sync-recalls")]
        public async Task<IActionResult> SyncRecalls(Guid id)
        {
            var count = await _recalls.SyncRecallsForEquipmentAsync(id);
            return Ok(new { Message = "Sync complete", NewRecallsFound = count });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EquipmentDto>> Get(Guid id)
        {
            var x = await _db.Equipments
                .Include(e => e.EquipmentType)
                .Include(e => e.FleetCategory)
                .Include(e => e.Recalls)
                    .ThenInclude(r => r.RecallCampaign)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (x == null) return NotFound();

            return Ok(new EquipmentDto
            {
                Id = x.Id,
                EquipmentTypeId = x.EquipmentTypeId,
                EquipmentTypeName = x.EquipmentType.Name,
                EquipmentTypeCode = x.EquipmentType.Code ?? "",
                FleetCategoryId = x.FleetCategoryId,
                FleetCategoryName = x.FleetCategory.Name,
                UnitNumber = x.UnitNumber,
                DisplayName = x.DisplayName,
                Vin = x.Vin,
                SerialNumber = x.SerialNumber,
                PlateNumber = x.PlateNumber,
                Make = x.Make,
                Model = x.Model,
                Year = x.Year,
                LifecycleStatus = x.LifecycleStatus,
                OperationalStatus = x.OperationalStatus,
                OdometerCurrent = x.OdometerCurrent,
                HoursCurrent = x.HoursCurrent,
                AcquiredDate = x.AcquiredDate,
                InServiceDate = x.InServiceDate,
                OutOfServiceDate = x.OutOfServiceDate,
                Notes = x.Notes,
                Recalls = x.Recalls.Select(r => new EquipmentRecallDto
                {
                    Id = r.Id,
                    CampaignCode = r.RecallCampaign.Code,
                    CampaignTitle = r.RecallCampaign.Title,
                    CampaignDescription = r.RecallCampaign.Description,
                    Manufacturer = r.RecallCampaign.Manufacturer,
                    IssueDate = r.RecallCampaign.IssueDate,
                    Status = r.Status,
                    FirstSeenAt = r.FirstSeenAt,
                    ResolvedAt = r.ResolvedAt,
                    Notes = r.Notes
                }).ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult<EquipmentDto>> Create(CreateEquipmentDto dto)
        {
            // Check uniqueness of UnitNumber within Tenant (Globally filtered)
            if (await _db.Equipments.AnyAsync(e => e.UnitNumber == dto.UnitNumber))
            {
                return Conflict("Unit Number already exists.");
            }

            var entity = new Equipment
            {
                EquipmentTypeId = dto.EquipmentTypeId,
                FleetCategoryId = dto.FleetCategoryId,
                UnitNumber = dto.UnitNumber,
                DisplayName = dto.DisplayName,
                Vin = dto.Vin,
                SerialNumber = dto.SerialNumber,
                PlateNumber = dto.PlateNumber,
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                LifecycleStatus = dto.LifecycleStatus,
                OperationalStatus = dto.OperationalStatus,
                OdometerCurrent = dto.OdometerCurrent,
                HoursCurrent = dto.HoursCurrent,
                AcquiredDate = dto.AcquiredDate,
                InServiceDate = dto.InServiceDate,
                Notes = dto.Notes
            };

            _db.Equipments.Add(entity);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateEquipmentDto dto)
        {
            var entity = await _db.Equipments.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return NotFound();

            // Check duplicate unit number if changed
            if (entity.UnitNumber != dto.UnitNumber)
            {
                if (await _db.Equipments.AnyAsync(e => e.UnitNumber == dto.UnitNumber))
                    return Conflict("Unit Number already exists.");
            }

            entity.EquipmentTypeId = dto.EquipmentTypeId;
            entity.FleetCategoryId = dto.FleetCategoryId;
            entity.UnitNumber = dto.UnitNumber;
            entity.DisplayName = dto.DisplayName;
            entity.Vin = dto.Vin;
            entity.SerialNumber = dto.SerialNumber;
            entity.PlateNumber = dto.PlateNumber;
            entity.Make = dto.Make;
            entity.Model = dto.Model;
            entity.Year = dto.Year;
            entity.LifecycleStatus = dto.LifecycleStatus;
            entity.OperationalStatus = dto.OperationalStatus;
            entity.OdometerCurrent = dto.OdometerCurrent;
            entity.HoursCurrent = dto.HoursCurrent;
            entity.AcquiredDate = dto.AcquiredDate;
            entity.InServiceDate = dto.InServiceDate;
            entity.OutOfServiceDate = dto.OutOfServiceDate;
            entity.Notes = dto.Notes;
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _db.Equipments.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return NotFound();

            // Soft delete
            entity.IsDeleted = true;
            entity.DeletedAt = DateTimeOffset.UtcNow;
            // entity.DeletedBy = ... (if using user context)

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
