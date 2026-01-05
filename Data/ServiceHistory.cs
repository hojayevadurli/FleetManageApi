using System.Text.Json;

namespace FleetManage.Api.Data
{
    public class ServiceHistory : TenantEntity
    {
        public Guid Id { get; set; }

        // ✅ Soft delete (match WorkOrder)
        public int? DeletedStatus { get; set; } = 0; // 0=active, 1=deleted
        public DateTime? DeletedAt { get; set; }

        // Equipment
        public Guid EquipmentId { get; set; }
        public Equipment? Equipment { get; set; }

        // Optional link to work order (if invoice came from WO)
        public Guid? WorkOrderId { get; set; }
        public WorkOrder? WorkOrder { get; set; }

        // Vendor
        public Guid? VendorId { get; set; }
        public ServicePartner? ServicePartner { get; set; }
        public string? VendorNameRaw { get; set; }

        // Invoice fields
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }

        // Usage
        public int? Odometer { get; set; }

        // Amounts
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }

        // Summary / category
        public string? Summary { get; set; }
        public string Category { get; set; } = "maintenance"; // maintenance/tires/inspection/other

        // Status
        public string Status { get; set; } = "confirmed"; // draft/needs_review/confirmed

        // Optional: store AI output snapshot (handy even if Document stores it)
        public JsonDocument? ExtractedJson { get; set; }
        public decimal? ConfidenceScore { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<ServiceHistoryLine> Lines { get; set; } = new();
    }

    public class ServiceHistoryLine : TenantEntity
    {
        public Guid Id { get; set; }

        public Guid ServiceHistoryId { get; set; }
        public ServiceHistory? ServiceHistory { get; set; }

        public string Type { get; set; } = "misc"; // part/labor/fee/misc
        public string Description { get; set; } = default!;

        public decimal Qty { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }

        public string? PartNumber { get; set; }
    }


}
