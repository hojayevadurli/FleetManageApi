using FleetManage.Api.Data;
using FleetManage.Api.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace FleetManage.Api.Services
{
    public class StripeService : IStripeService
    {
        private readonly string _webhookSecret;

        public StripeService(IConfiguration config)
        private readonly ILogger<StripeService> _logger; // Added logger field

        public StripeService(IConfiguration config, ILogger<StripeService> logger) // Injected ILogger
        {
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
            _webhookSecret = config["Stripe:WebhookSecret"] 
                             ?? throw new Exception("Stripe:WebhookSecret not configured");
            _logger = logger; // Assigned logger
        }

        public async Task<Customer> CreateCustomerAsync(Tenant tenant)
        {
            var options = new CustomerCreateOptions
            {
                Email = tenant.Email,
                Name = tenant.Name,
                Metadata = new Dictionary<string, string>
                {
                    { "TenantId", tenant.Id.ToString() }
                }
            };
            var service = new CustomerService();
            return await service.CreateAsync(options);
        }

        public async Task<Session> CreateCheckoutSessionAsync(Tenant tenant, string priceId, string successUrl, string cancelUrl)
        {
            var options = new SessionCreateOptions
            {
                Customer = tenant.StripeCustomerId,
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = priceId,
                        Quantity = 1,
                    },
                },
                Mode = "subscription",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                SubscriptionData = new SessionSubscriptionDataOptions
                {
                    Metadata = new Dictionary<string, string>
                    {
                        { "TenantId", tenant.Id.ToString() }
                    }
                },
                ClientReferenceId = tenant.Id.ToString()
            };

            var service = new SessionService();
            return await service.CreateAsync(options);
        }

        public async Task<Event> ConstructEventAsync(string json, string stripeSignature)
        {
            // Throws exception if signature verification fails
            return EventUtility.ConstructEvent(json, stripeSignature, _webhookSecret);
        }
    }
}
