namespace FleetManage.Api.DTOs
{
    public class ServiceHistoryUpsertDto
    {
        public Guid EquipmentId { get; set; }

        public Guid? WorkOrderId { get; set; }
        public Guid? VendorId { get; set; }

        public string? VendorNameRaw { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }

        public int? Odometer { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }

        public string? Summary { get; set; }
        public string Category { get; set; } = "maintenance";

        public string Status { get; set; } = "needs_review";

        public List<ServiceHistoryLineUpsertDto> Lines { get; set; } = new();
    }

    public class ServiceHistoryLineUpsertDto
    {
        public string Type { get; set; } = "misc";
        public string Description { get; set; } = default!;
        public decimal Qty { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public string? PartNumber { get; set; }
    }

}
