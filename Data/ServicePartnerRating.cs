using System.ComponentModel.DataAnnotations;

namespace FleetManage.Api.Data
{
    public class ServicePartnerRating : TenantEntity
    {
        public Guid ServicePartnerId { get; set; }
        public ServicePartner ServicePartner { get; set; } = default!;

        [Range(1, 5)]
        public int Rating { get; set; } // 1-5 Stars
        
        public string? ReviewText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
