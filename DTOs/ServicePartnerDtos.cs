using System.ComponentModel.DataAnnotations;

namespace FleetManage.Api.DTOs
{
    public class ServicePartnerDtos
    {
        public record ServicePartnerDto(
            Guid Id,
            string Name,
            
            string Address1,
            string? Address2,
            string City,
            string State,
            string PostalCode,
            string Country,

            string? Phone,
            string? Email,
            string? Website,
            string? ContactName,
            
            int? IndustryId,
            string Type,    // "Shop", "Mobile", etc.

            decimal LaborRate,
            List<string> Specialties,   // JSON array
            
            double? Latitude,
            double? Longitude,

            string NetworkTier,         // "Preferred", "Standard", "Warning"
            string PricingStrategy,     // $, $$, $$$
            decimal? AuditScore,

            decimal AverageRating,
            int ReviewCount,
            
            bool IsActive,
            string? Notes,
            DateTime CreatedAt
        );

        public record CreateServicePartnerDto(
            [Required] string Name,
            
            [Required] string Address1,
            string? Address2,
            [Required] string City,
            [Required] string State,
            [Required] string PostalCode,
            string? Country, // Removed default value to avoid ordering issues

            string? Phone,
            string? Email,
            string? Website,
            string? ContactName,
            
            int? IndustryId,
            string? Type, // "Shop", "Mobile"...

            decimal LaborRate,
            List<string>? Specialties,

            string? NetworkTier,
            string? PricingStrategy,
            string? Notes
        );

        public record UpdateServicePartnerDto(
            string Name,
            
            string Address1,
            string? Address2,
            string City,
            string State,
            string PostalCode,
            string Country,

            string? Phone,
            string? Email,
            string? Website,
            string? ContactName,
            
            int? IndustryId,
            string? Type,

            decimal LaborRate,
            List<string>? Specialties,

            string? NetworkTier,
            string? PricingStrategy,
            
            bool IsActive,
            string? Notes
        );

        public record ServicePartnerRatingDto(
            Guid Id,
            Guid ServicePartnerId,
            int Rating,
            string? ReviewText,
            DateTime CreatedAt
        );

        public record CreateRatingDto(
            [Range(1,5)] int Rating,
            string? ReviewText
        );
    }
}
