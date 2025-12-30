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
    }

    public class WorkOrderLine: TenantEntity
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

        // ✅ Soft delete
        public int? DeletedStatus { get; set; } = 0; // 0=active, 1=deleted
        public DateTime? DeletedAt { get; set; }    // optional audit
        public string AssetType { get; set; } = "truck"; // truck/trailer
        public Guid AssetId { get; set; }

        public Guid? VendorId { get; set; }
        public string? WoNumber { get; set; }

        public int? Odometer { get; set; }
        public DateTime ServiceDate { get; set; } = DateTime.UtcNow.Date;

        public string? Summary { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }

        public string Status { get; set; } = "draft"; // draft/open/closed/paid

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<WorkOrderLine> Lines { get; set; } = new();
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



