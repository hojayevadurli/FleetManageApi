using Microsoft.AspNetCore.Identity;

namespace FleetManage.Api.Data
{
    public class AppUser : IdentityUser
    {
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = default!;
    }
}

