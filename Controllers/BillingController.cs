using FleetManage.Api.Data;
using FleetManage.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace FleetManage.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillingController : ControllerBase
    {
        private readonly IStripeService _stripeService;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;
        private readonly ILogger<BillingController> _logger;

        public BillingController(IStripeService stripeService, UserManager<AppUser> userManager, AppDbContext db, ILogger<BillingController> logger)
        {
            _stripeService = stripeService;
            _userManager = userManager;
            _db = db;
            _logger = logger;
        }

        [HttpPost("checkout-session")]
        [Authorize]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == user.TenantId);
            if (tenant == null) return NotFound("Tenant not found");

            // Ensure Stripe customer exists
            if (string.IsNullOrEmpty(tenant.StripeCustomerId))
            {
                var customer = await _stripeService.CreateCustomerAsync(tenant);
                tenant.StripeCustomerId = customer.Id;
                await _db.SaveChangesAsync();
            }

            var successUrl = request.SuccessUrl ?? $"{Request.Scheme}://{Request.Host}/billing/success";
            var cancelUrl = request.CancelUrl ?? $"{Request.Scheme}://{Request.Host}/billing/cancel";

            var session = await _stripeService.CreateCheckoutSessionAsync(tenant, request.PriceId, successUrl, cancelUrl);

            return Ok(new { sessionId = session.Id, url = session.Url });
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];

            try
            {
                var stripeEvent = await _stripeService.ConstructEventAsync(json, signature);

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    await HandleCheckoutSessionCompleted(session);
                }
                else if (stripeEvent.Type == "customer.subscription.updated")
                {
                    var subscription = stripeEvent.Data.Object as Stripe.Subscription;
                    await HandleSubscriptionUpdated(subscription);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stripe Webhook Error");
                return BadRequest();
            }
        }

        private async Task HandleCheckoutSessionCompleted(Stripe.Checkout.Session session)
        {
            if (session.ClientReferenceId == null) return;

            if (Guid.TryParse(session.ClientReferenceId, out var tenantId))
            {
                var tenant = await _db.Tenants.FindAsync(tenantId);
                if (tenant != null)
                {
                    // Auto-onboarding logic as requested
                    if (tenant.OnboardingCompletedAt == null)
                    {
                        tenant.OnboardingCompletedAt = DateTimeOffset.UtcNow;
                        tenant.Status = TenantStatus.Active;
                    }

                    tenant.StripeSubscriptionId = session.SubscriptionId;
                    tenant.BillingStatus = "active"; // optimistic update
                    await _db.SaveChangesAsync();
                }
            }
        }

        private async Task HandleSubscriptionUpdated(Stripe.Subscription subscription)
        {
            var customerId = subscription.CustomerId;
            var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.StripeCustomerId == customerId);
            
            if (tenant != null)
            {
                tenant.BillingStatus = subscription.Status;
                // tenant.CurrentPeriodEnd = subscription.CurrentPeriodEnd; // Build ERROR: Property not found?
                tenant.TrialEndsAt = subscription.TrialEnd;
                tenant.StripePriceId = subscription.Items.Data.FirstOrDefault()?.Price.Id;
                
                // Ensure status is active if billing is okay
                if (tenant.Status == TenantStatus.Pending && 
                   (subscription.Status == "active" || subscription.Status == "trialing"))
                {
                     if (tenant.OnboardingCompletedAt == null)
                     {
                        tenant.OnboardingCompletedAt = DateTimeOffset.UtcNow;
                     }
                     tenant.Status = TenantStatus.Active;
                }

                await _db.SaveChangesAsync();
            }
        }
    }

    public class CreateCheckoutRequest
    {
        public string PriceId { get; set; } = default!;
        public string? SuccessUrl { get; set; }
        public string? CancelUrl { get; set; }
    }
}
