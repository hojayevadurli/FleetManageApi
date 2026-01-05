using FleetManage.Api.Data;
using FleetManage.Api.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManage.Api.Data
{
    public class ServicePartner : TenantEntity
    {
        public string Name { get; set; } = default!;
        
        // Structured Address
        public string Address1 { get; set; } = default!;
        public string? Address2 { get; set; }
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string PostalCode { get; set; } = default!;
        public string Country { get; set; } = "USA";

        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? ContactName { get; set; }

        // Classification
        public int? IndustryId { get; set; }
        public Industry? Industry { get; set; }

        public ServicePartnerType Type { get; set; } = ServicePartnerType.Shop;

        // Core Business Fields
        public decimal LaborRate { get; set; }
        
        // JSON array of strings e.g. ["Brakes", "Diesel Engine", "Tires"]
        [Column(TypeName = "jsonb")]
        public List<string> Specialties { get; set; } = new();

        // Location
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Tier / Strategy
        public ServicePartnerTier NetworkTier { get; set; } = ServicePartnerTier.Standard;
        public string PricingStrategy { get; set; } = "$$"; // $, $$, $$$

        // Performance / Audit
        public decimal? AuditScore { get; set; }
        
        // Aggregated Rating Info (Updated via Domain Events or simple service method)
        public decimal AverageRating { get; set; } = 0;
        public int ReviewCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
        public ICollection<ServicePartnerRating> Ratings { get; set; } = new List<ServicePartnerRating>();
    }
}
