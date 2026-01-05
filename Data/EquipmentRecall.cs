using FleetManage.Api.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManage.Api.Data
{
    public class EquipmentRecall : TenantEntity
    {
        public Guid Id { get; set; }

        public Guid EquipmentId { get; set; }
        public Equipment Equipment { get; set; } = default!;

        public Guid RecallCampaignId { get; set; }
        public RecallCampaign RecallCampaign { get; set; } = default!;

        public EquipmentRecallStatus Status { get; set; } = EquipmentRecallStatus.Open;

        public DateTimeOffset FirstSeenAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? LastCheckedAt { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }

        public string? Notes { get; set; }
    }
}
