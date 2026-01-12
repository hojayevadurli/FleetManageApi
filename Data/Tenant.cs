using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FleetManage.Api.Data
{
  
   public enum TenantStatus
    {
        Pending = 0,     // created but not fully onboarded
        Active = 1,      // allowed to use app (subject to billing)
        Suspended = 2,   // admin blocked
        Closed = 3       // permanently closed
    }

   public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public int? IndustryId { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Lifecycle
    public TenantStatus Status { get; set; } = TenantStatus.Pending;
    public DateTimeOffset? OnboardingCompletedAt { get; set; }
    public DateTimeOffset? DeactivatedAt { get; set; }
    public DateTimeOffset? SuspendedAt { get; set; }
    public string? SuspendedReason { get; set; }
    public string? Notes { get; set; }              // internal
    public DateTimeOffset? LastActivityAt { get; set; }

    // Stripe billing
    public string? StripeCustomerId { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public string? StripeSubscriptionItemId { get; set; }
    public string BillingStatus { get; set; } = "inactive"; // inactive|trialing|active|past_due|canceled|unpaid
    public DateTimeOffset? CurrentPeriodEnd { get; set; }
    public DateTimeOffset? TrialEndsAt { get; set; }
    public string? StripePriceId { get; set; }
    public string? PlanKey { get; set; }

    public Industry? Industry { get; set; }
    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
}

    

    public abstract class TenantEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
    }
}
