using System.ComponentModel.DataAnnotations;

namespace FleetManage.Api.DTOs
{
    public class TenantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public int? IndustryId { get; set; }
        public string? IndustryName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateTenantDto
    {
        [Required]
        public string Name { get; set; } = default!;
        
        public int? IndustryId { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        [Phone]
        public string? Phone { get; set; }
    }
}
