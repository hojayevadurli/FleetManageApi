using FleetManage.Api.Data;
using FleetManage.Api.DTOs;
using FleetManage.Api.Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetManage.Api.Controllers
{
    [ApiController]
    [Route("api/equipment-recalls")]
    public class EquipmentRecallsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EquipmentRecallsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("equipment/{equipmentId}")]
        public async Task<ActionResult<IEnumerable<EquipmentRecallDto>>> GetByEquipmentId(Guid equipmentId)
        {
            var recalls = await _context.EquipmentRecalls
                .Include(r => r.RecallCampaign)
                .Where(r => r.EquipmentId == equipmentId)
                .OrderByDescending(r => r.FirstSeenAt)
                .ToListAsync();

            var dtos = recalls.Select(r => new EquipmentRecallDto
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
            });

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EquipmentRecallDto>> GetById(Guid id)
        {
            var recall = await _context.EquipmentRecalls
                .Include(r => r.RecallCampaign)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recall == null)
            {
                return NotFound();
            }

            var dto = new EquipmentRecallDto
            {
                Id = recall.Id,
                CampaignCode = recall.RecallCampaign.Code,
                CampaignTitle = recall.RecallCampaign.Title,
                CampaignDescription = recall.RecallCampaign.Description,
                Manufacturer = recall.RecallCampaign.Manufacturer,
                IssueDate = recall.RecallCampaign.IssueDate,

                Status = recall.Status,
                FirstSeenAt = recall.FirstSeenAt,
                ResolvedAt = recall.ResolvedAt,
                Notes = recall.Notes
            };

            return Ok(dto);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EquipmentRecallUpdateDto updateDto)
        {
            var recall = await _context.EquipmentRecalls.FindAsync(id);
            if (recall == null)
            {
                return NotFound();
            }

            recall.Status = updateDto.Status;
            recall.Notes = updateDto.Notes;

            if (updateDto.ResolvedAt.HasValue)
            {
                recall.ResolvedAt = updateDto.ResolvedAt;
            }
            // Auto-set ResolvedAt if status seems resolved? 
            // Let's stick to explicit for now, but if status is newly 'completed' like, maybe set resolved date?
            // Actually, best to let the client decide the date or the user interaction.

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
