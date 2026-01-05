using FleetManage.Api.Data.Enums;

namespace FleetManage.Api.DTOs
{
    public class EquipmentRecallDto
    {
        public Guid Id { get; set; }
        
        // Flattened Campaign Info
        public string CampaignCode { get; set; } = default!;
        public string CampaignTitle { get; set; } = default!;
        public string? CampaignDescription { get; set; }
        public string Manufacturer { get; set; } = default!;
        public DateOnly IssueDate { get; set; }

        public EquipmentRecallStatus Status { get; set; }
        public string StatusName => Status.ToString();

        public DateTimeOffset FirstSeenAt { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }
        public string? Notes { get; set; }
    }
    public class EquipmentRecallUpdateDto
    {
        public EquipmentRecallStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }
    }
}
