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

        private readonly ILogger<StripeService> _logger;

        public StripeService(IConfiguration config, ILogger<StripeService> logger)
        {
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
            _webhookSecret = config["Stripe:WebhookSecret"] 
                             ?? throw new Exception("Stripe:WebhookSecret not configured");
            _logger = logger;
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

        public Task<Event> ConstructEventAsync(string json, string stripeSignature)
        {
            try 
            {
                // Throws exception if signature verification fails
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _webhookSecret);
                return Task.FromResult(stripeEvent);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Stripe signature verification failed");
                throw;
            }
        }
    }
}
