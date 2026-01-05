using FleetManage.Api.Data.Enums;
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
        public Guid? EquipmentId { get; set; }

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
        bool IsDeleted,
        DateTimeOffset? DeletedAt,
        Guid EquipmentId,
        Guid? VendorId,
        string? WorkOrderNumber,
        int? OdometerAtService,
        DateTimeOffset OpenedAt,
        DateTimeOffset? ClosedAt,
        string? Title,
        string? Complaint,
        string? Diagnosis,
        string? Resolution,
        string? Notes,
        decimal? EstimatedTotal,
        decimal? ManualActualTotal,
        string Status,        // Enum string
        string Priority,      // Enum string
        string CostSource,    // Enum string
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
        [Required]
        public Guid EquipmentId { get; set; }

        public Guid? VendorId { get; set; }

        [StringLength(64)]
        public string? WorkOrderNumber { get; set; }

        [Range(0, int.MaxValue)]
        public int? OdometerAtService { get; set; }

        [Required]
        public DateTimeOffset OpenedAt { get; set; }

        [StringLength(200)]
        public string? Title { get; set; }

        public string? Complaint { get; set; }

        [Range(0, 999999999)]
        public decimal? EstimatedTotal { get; set; }
        
        [Range(0, 999999999)]
        public decimal? ManualActualTotal { get; set; }

        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Open;
        public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Normal;
        public WorkOrderCostSource CostSource { get; set; } = WorkOrderCostSource.Estimated;

        [Required]
        public List<CreateWorkOrderLineDto> Lines { get; set; } = new();

        // One-time doc linking
        public List<Guid>? DocumentIds { get; set; } = null;
        public bool ReplaceDocuments { get; set; } = false;
    }

    public class UpdateWorkOrderDto
    {
        public Guid EquipmentId { get; set; }

        public Guid? VendorId { get; set; }

        [StringLength(64)]
        public string? WorkOrderNumber { get; set; }

        [Range(0, int.MaxValue)]
        public int? OdometerAtService { get; set; }
        public int? HoursAtService { get; set; }

        public DateTimeOffset OpenedAt { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }

        public string? Title { get; set; }
        public string? Complaint { get; set; }
        public string? Diagnosis { get; set; }
        public string? Resolution { get; set; }
        public string? Notes { get; set; }

        public WorkOrderStatus Status { get; set; }
        public WorkOrderPriority Priority { get; set; }
        public WorkOrderCostSource CostSource { get; set; }

        public decimal? EstimatedTotal { get; set; }
        public decimal? ManualActualTotal { get; set; }

        [Required]
        public List<CreateWorkOrderLineDto> Lines { get; set; } = new();

        public List<Guid>? DocumentIds { get; set; } = null;
        public bool ReplaceDocuments { get; set; } = false;
    }
}
