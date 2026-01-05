using FleetManage.Api.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManage.Api.Data
{
    public class EquipmentDocument : TenantEntity
    {
        public Guid Id { get; set; } // Explicit Id if TenantEntity requires it, though usually Join tables use composite keys. But TenantEntity implies Id. User snippet shows TenantEntity inheritance.

        public Guid EquipmentId { get; set; }
        public Equipment Equipment { get; set; } = default!;

        public Guid DocumentId { get; set; }
        public Document Document { get; set; } = default!;

        public EquipmentDocRole DocRole { get; set; } = EquipmentDocRole.General;
        public DateTimeOffset AddedAt { get; set; } = DateTimeOffset.UtcNow;

        public Guid? AddedBy { get; set; }
    }
}
