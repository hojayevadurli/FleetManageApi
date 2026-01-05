using System.ComponentModel.DataAnnotations;

namespace FleetManage.Api.DTOs
{
    public class CreateWorkOrderFromDocumentDTO
    {
        [Required]
        public Guid EquipmentId { get; set; }

        public Guid? VendorId { get; set; } // optional override

        // Optional overrides if you want to set these manually:
        [Range(0, int.MaxValue)]
        public int? Odometer { get; set; }

        public DateTime? ServiceDate { get; set; }

        [StringLength(1000)]
        public string? Summary { get; set; }

        public string? InvoiceNumber { get; set; }
        public decimal? TaxAmount { get; set; }
        public string? Category { get; set; } = "maintenance";
        public string? VendorNameRaw { get; set; }

        // If true: mark doc confirmed after creating WO (otherwise keep needs_review)
        public bool ConfirmDocument { get; set; } = true;
    }
}
