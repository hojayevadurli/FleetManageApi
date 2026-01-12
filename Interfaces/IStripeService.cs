using FleetManage.Api.Data;
using Stripe;
using Stripe.Checkout;

namespace FleetManage.Api.Interfaces
{
    public interface IStripeService
    {
        Task<Customer> CreateCustomerAsync(Tenant tenant);
        Task<Session> CreateCheckoutSessionAsync(Tenant tenant, string priceId, string successUrl, string cancelUrl);
        Task<Event> ConstructEventAsync(string json, string stripeSignature);
    }
}
