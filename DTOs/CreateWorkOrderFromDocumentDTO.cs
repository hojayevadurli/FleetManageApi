using System.ComponentModel.DataAnnotations;

public class CreateWorkOrderFromDocumentDTO
{
    [Required, StringLength(16)]
    public string AssetType { get; set; } = default!; // truck/trailer

    [Required]
    public Guid AssetId { get; set; }

    public Guid? VendorId { get; set; } // optional override

    // Optional overrides if you want to set these manually:
    [Range(0, int.MaxValue)]
    public int? Odometer { get; set; }

    public DateTime? ServiceDate { get; set; }

    [StringLength(1000)]
    public string? Summary { get; set; }

    // If true: mark doc confirmed after creating WO (otherwise keep needs_review)
    public bool ConfirmDocument { get; set; } = true;
}
