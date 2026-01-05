using FleetManage.Api.Data.Enums;
using System;

namespace FleetManage.Api.Data
{
    public class WorkOrderDocument : TenantEntity
    {
        public Guid Id { get; set; } // Needed for TenantEntity usually, or configured as composite key? 
        // User provided: public class WorkOrderDocument : TenantEntity
        // TenantEntity usually has public Guid Id { get; set; } if it's the base.
        // Let's check TenantEntity definition. But standard is Id.

        public Guid WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; set; } = default!;

        public Guid DocumentId { get; set; }
        public Document Document { get; set; } = default!;

        public WorkOrderDocRole DocRole { get; set; } = WorkOrderDocRole.General;
        public DateTimeOffset AddedAt { get; set; } = DateTimeOffset.UtcNow;

        public Guid? AddedBy { get; set; }
    }
}
