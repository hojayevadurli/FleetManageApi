using System.ComponentModel.DataAnnotations;

namespace FleetManage.Api.DTOs
{
    public class EquipmentDTOs
    {
    }

    public record TruckDto(
    Guid Id,
    string Number,
    string Vin,
    int? Year,
    string? Make,
    string? Model,
    DateTime? PurchasedAt,
    string? PlateNumber,
    double? Mileage,
    string? EngineType,
    string? Status
);

    public class CreateTruckDto
    {
        [Required, StringLength(32)]
        public string Number { get; set; } = default!;

        [Required, StringLength(32)]
        public string Vin { get; set; } = default!;

        public int? Year { get; set; }

        [StringLength(64)]
        public string? Make { get; set; }

        [StringLength(64)]
        public string? Model { get; set; }

        public DateTime? PurchasedAt { get; set; }

        [StringLength(32)]
        public string? PlateNumber { get; set; }

        public double? Mileage { get; set; }

        [StringLength(64)]
        public string? EngineType { get; set; }

        [StringLength(32)]
        public string? Status { get; set; }
    }

    public class UpdateTruckDto
    {
        [Required, StringLength(32)]
        public string Number { get; set; } = default!;

        [Required, StringLength(32)]
        public string Vin { get; set; } = default!;

        public int? Year { get; set; }

        [StringLength(64)]
        public string? Make { get; set; }

        [StringLength(64)]
        public string? Model { get; set; }

        public DateTime? PurchasedAt { get; set; }

        [StringLength(32)]
        public string? PlateNumber { get; set; }

        public double? Mileage { get; set; }

        [StringLength(64)]
        public string? EngineType { get; set; }

        [StringLength(32)]
        public string? Status { get; set; }
    }

    // -------- TRAILER DTOS --------

    public record TrailerDto(
        Guid Id,
        string Number,
        string Vin,
        int? Year,
        string? Make,
        string? Model,
        DateTime? PurchasedAt,
        string? Type,
        double? Length,
        double? WeightCapacity,
        string? Status
    );

    public class CreateTrailerDto
    {
        [Required, StringLength(32)]
        public string Number { get; set; } = default!;

        [Required, StringLength(32)]
        public string Vin { get; set; } = default!;

        public int? Year { get; set; }

        [StringLength(64)]
        public string? Make { get; set; }

        [StringLength(64)]
        public string? Model { get; set; }

        public DateTime? PurchasedAt { get; set; }

        [StringLength(32)]
        public string? Type { get; set; }

        public double? Length { get; set; }

        public double? WeightCapacity { get; set; }

        [StringLength(32)]
        public string? Status { get; set; }
    }

    public class UpdateTrailerDto
    {
        [Required, StringLength(32)]
        public string Number { get; set; } = default!;

        [Required, StringLength(32)]
        public string Vin { get; set; } = default!;

        public int? Year { get; set; }

        [StringLength(64)]
        public string? Make { get; set; }

        [StringLength(64)]
        public string? Model { get; set; }

        public DateTime? PurchasedAt { get; set; }

        [StringLength(32)]
        public string? Type { get; set; }

        public double? Length { get; set; }

        public double? WeightCapacity { get; set; }

        [StringLength(32)]
        public string? Status { get; set; }
    }
}
