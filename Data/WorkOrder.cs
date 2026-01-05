using FleetManage.Api.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace FleetManage.Api.Data
{

    public class Document : TenantEntity
    {
        public Guid Id { get; set; }

        public Guid UploadedBy { get; set; }  // AppUser.Id

        public string FileUrl { get; set; } = default!;
        public string FileType { get; set; } = default!; // pdf/jpg/png/heic

        public string DocKind { get; set; } = "unknown"; // invoice/receipt/work_order/unknown
        public string? VendorNameRaw { get; set; }

        public string Status { get; set; } = "uploaded"; // uploaded/extracting/needs_review/confirmed/failed

        public JsonDocument? ExtractedJson { get; set; }
        public decimal? ConfidenceScore { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<EquipmentDocument> EquipmentDocuments { get; set; } = new List<EquipmentDocument>();
        public ICollection<WorkOrderDocument> WorkOrderDocuments { get; set; } = new List<WorkOrderDocument>();
    }

    public class WorkOrderLineItem : TenantEntity
    {
        public Guid Id { get; set; }

        public Guid WorkOrderId { get; set; }
        public WorkOrder? WorkOrder { get; set; } 

        public string Type { get; set; } = "misc"; // part/labor/fee/misc
        public string Description { get; set; } = default!;

        public decimal Qty { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }

        public string? PartNumber { get; set; }
    }


    public class WorkOrder : TenantEntity
    {
        public Guid Id { get; set; }

        public Guid EquipmentId { get; set; }
        public Equipment Equipment { get; set; } = default!;

        public Guid? VendorId { get; set; }
        public ServicePartner? Vendor { get; set; } // Mapped to ServicePartner

        public string? WorkOrderNumber { get; set; }

        public WorkOrderStatus Status { get; set; }
        public WorkOrderPriority Priority { get; set; }

        public DateTimeOffset OpenedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ClosedAt { get; set; }

        public string? Title { get; set; }
        public string? Complaint { get; set; }
        public string? Diagnosis { get; set; }
        public string? Resolution { get; set; }
        public string? Notes { get; set; }

        public int? OdometerAtService { get; set; }
        public int? HoursAtService { get; set; }

        public WorkOrderCostSource CostSource { get; set; }
        public decimal? EstimatedTotal { get; set; }
        public decimal? ManualActualTotal { get; set; }

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; } // AppUser Id

        public ICollection<WorkOrderLineItem> LineItems { get; set; } = new List<WorkOrderLineItem>();
        public ICollection<WorkOrderDocument> WorkOrderDocuments { get; set; } = new List<WorkOrderDocument>();
        
        // Backward compatibility / Helper (Optional, but fields removed per request)
    }

    public class DocumentLink : TenantEntity
    {
        public Guid Id { get; set; } 
        public Guid DocumentId { get; set; }
        public Document Document { get; set; } = default!;

        public string EntityType { get; set; } = "asset"; // work_order/asset/vendor
        public Guid EntityId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }


}
