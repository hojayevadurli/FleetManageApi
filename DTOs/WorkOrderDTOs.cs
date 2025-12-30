using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace FleetManage.Api.DTOs
{
    // -------- DOCUMENT DTOS --------

    public record DocumentDto(
        Guid Id,
        string FileUrl,
        string FileType,
        string DocKind,
        string? VendorNameRaw,
        string Status,
        JsonDocument? ExtractedJson,
        decimal? ConfidenceScore,
        DateTime CreatedAt
    );

    public class CreateDocumentDto
    {
        // v1: client uploads to storage and sends the file url here
        [Required, StringLength(2048)]
        public string FileUrl { get; set; } = default!;

        [Required, StringLength(64)]
        public string FileType { get; set; } = default!; // store content-type (e.g. image/jpeg, video/mp4, application/pdf)

        [StringLength(32)]
        public string DocKind { get; set; } = "unknown"; // invoice/receipt/work_order/unknown

        [StringLength(128)]
        public string? VendorNameRaw { get; set; }

        // Optional: link document to an asset immediately
        [StringLength(16)]
        public string? AssetType { get; set; } // truck/trailer
        public Guid? AssetId { get; set; }

        public bool RunAiExtract { get; set; } = true;
    }

    // -------- WORK ORDER DTOS --------

    public record WorkOrderLineDto(
        Guid Id,
        string Type,          // part/labor/fee/misc
        string Description,
        decimal Qty,
        decimal UnitPrice,
        decimal Amount,
        string? PartNumber
    );

    public record WorkOrderDocumentDto(
        Guid Id,
        string FileUrl,
        string FileType,
        string DocKind,
        string Status,
        decimal? ConfidenceScore,
        DateTime CreatedAt
    );

    public record WorkOrderDto(
        Guid Id,
         int? DeletedStatus,     // 0=active, 1=deleted
        DateTime? DeletedAt,    // audit timestamp (nullable)
        string AssetType,     // truck/trailer
        Guid AssetId,
        Guid? VendorId,
        string? WoNumber,
        int? Odometer,
        DateTime ServiceDate,
        string? Summary,
        decimal TotalAmount,
        decimal TaxAmount,
        string Status,        // draft/open/closed/paid
        List<WorkOrderLineDto> Lines,
        List<WorkOrderDocumentDto> Documents
    );

    public class CreateWorkOrderLineDto
    {
        [Required, StringLength(16)]
        public string Type { get; set; } = "misc"; // part/labor/fee/misc

        [Required, StringLength(512)]
        public string Description { get; set; } = default!;

        [Range(0.000, 9999999)]
        public decimal Qty { get; set; } = 1;

        [Range(0, 999999999)]
        public decimal UnitPrice { get; set; }

        [Range(0, 999999999)]
        public decimal Amount { get; set; }

        [StringLength(64)]
        public string? PartNumber { get; set; }
    }

    public class CreateWorkOrderDto
    {
        [Required, StringLength(16)]
        public string AssetType { get; set; } = default!; // truck/trailer

        [Required]
        public Guid AssetId { get; set; }

        public Guid? VendorId { get; set; }

        [StringLength(64)]
        public string? WoNumber { get; set; }

        [Range(0, int.MaxValue)]
        public int? Odometer { get; set; }

        [Required]
        public DateTime ServiceDate { get; set; }

        [StringLength(1000)]
        public string? Summary { get; set; }

        [Range(0, 999999999)]
        public decimal TotalAmount { get; set; }

        [Range(0, 999999999)]
        public decimal TaxAmount { get; set; }

        [StringLength(16)]
        public string Status { get; set; } = "open"; // draft/open/closed/paid

        [Required]
        public List<CreateWorkOrderLineDto> Lines { get; set; } = new();

        // IMPORTANT:
        // null = do not attach anything / client didn't provide docs
        // []   = attach nothing (explicit)
        public List<Guid>? DocumentIds { get; set; } = null;

        // Optional safety: if false, backend will ignore DocumentIds
        public bool ReplaceDocuments { get; set; } = false;
    }

    public class UpdateWorkOrderDto
    {
        [Required, StringLength(16)]
        public string AssetType { get; set; } = default!;

        [Required]
        public Guid AssetId { get; set; }

        public Guid? VendorId { get; set; }

        [StringLength(64)]
        public string? WoNumber { get; set; }

        [Range(0, int.MaxValue)]
        public int? Odometer { get; set; }

        [Required]
        public DateTime ServiceDate { get; set; }

        [StringLength(1000)]
        public string? Summary { get; set; }

        [Range(0, 999999999)]
        public decimal TotalAmount { get; set; }

        [Range(0, 999999999)]
        public decimal TaxAmount { get; set; }

        [StringLength(16)]
        public string Status { get; set; } = "open";

        [Required]
        public List<CreateWorkOrderLineDto> Lines { get; set; } = new();

        // IMPORTANT:
        // null = keep existing attachments unchanged
        // []   = remove all attachments (only if ReplaceDocuments=true)
        public List<Guid>? DocumentIds { get; set; } = null;

        // Safety flag to avoid accidental wipes
        public bool ReplaceDocuments { get; set; } = false;
    }
}
