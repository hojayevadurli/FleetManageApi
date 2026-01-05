using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManage.Api.Data
{
    public class RecallCampaign : TenantEntity
    {
        public string Title { get; set; } = default!;
        public string Code { get; set; } = default!; // e.g. "ABC-123"
        public string Manufacturer { get; set; } = default!; // e.g. "Ford"
        
        public string? Description { get; set; }
        
        public DateOnly IssueDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<EquipmentRecall> EquipmentRecalls { get; set; } = new List<EquipmentRecall>();
    }
}
