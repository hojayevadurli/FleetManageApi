using FleetManage.Api.Data;
using FleetManage.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetManage.Api.Controllers
{
    [ApiController]
    [Route("api/equipment-types")]
    [AllowAnonymous] 
    public class EquipmentTypesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public EquipmentTypesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<EquipmentTypeDto>>> GetTypes(
            [FromQuery] int? industryId,
            [FromQuery] int? fleetCategoryId)
        {
            var query = _db.EquipmentTypes.AsNoTracking();

            if (industryId.HasValue)
                query = query.Where(x => x.IndustryId == industryId.Value);

            if (fleetCategoryId.HasValue)
                query = query.Where(x => x.FleetCategoryId == fleetCategoryId.Value);

            var list = await query
                .OrderBy(x => x.Name)
                .Select(x => new EquipmentTypeDto
                {
                    Id = x.Id,
                    IndustryId = x.IndustryId,
                    FleetCategoryId = x.FleetCategoryId,
                    Name = x.Name,
                    Code = x.Code,
                    MeterMode = x.MeterMode,
                    HasVin = x.HasVin,
                    HasSerial = x.HasSerial,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}
