using FleetManage.Api.Data;
using FleetManage.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FleetManage.Api.Middleware
{
    public sealed class TenantGuardMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantGuardMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, AppDbContext db, ITenantContext tenantContext)
        {
            // Always allow CORS preflight
            if (context.Request.Method == "OPTIONS")
            {
                await _next(context);
                return;
            }

            var path = (context.Request.Path.Value ?? "").ToLowerInvariant();

            // Public endpoints (adjust as needed)
            if (IsPublicPath(path))
            {
                await _next(context);
                return;
            }

            // If not authenticated, let [Authorize] / auth middleware handle 401
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            Guid tenantId;
            try
            {
                tenantId = tenantContext.TenantId; // may throw if claim missing/invalid (GOOD)
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Missing or invalid tenant context.");
                return;
            }

            // Load tenant
            var tenant = await db.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == tenantId);
            if (tenant is null)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Tenant not found.");
                return;
            }

            // Hard block
            if (tenant.DeactivatedAt != null || tenant.Status is TenantStatus.Suspended or TenantStatus.Closed)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Tenant is not active.");
                return;
            }

            // Onboarding gating
            if (tenant.Status == TenantStatus.Pending && !IsOnboardingAllowedPath(path))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Complete onboarding to continue.");
                return;
            }

            // Billing gating (block writes unless active/trialing)
            if (IsWriteMethod(context.Request.Method) && !IsBillingPath(path))
            {
                var isActive = string.Equals(tenant.BillingStatus, "active", StringComparison.OrdinalIgnoreCase);
                var isTrialing = string.Equals(tenant.BillingStatus, "trialing", StringComparison.OrdinalIgnoreCase);
                
                // If trialing, check if expired
                if (isTrialing && tenant.TrialEndsAt.HasValue && tenant.TrialEndsAt.Value < DateTimeOffset.UtcNow)
                {
                    isTrialing = false; // Trial expired
                }

                if (!isActive && !isTrialing)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Billing inactive or trial expired. Please update payment.");
                    return;
                }
            }

            // Update LastActivityAt efficiently (avoid SaveChanges tracking)
            await db.Tenants
                .Where(t => t.Id == tenantId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(t => t.LastActivityAt, DateTimeOffset.UtcNow));

            await _next(context);
        }

        private static bool IsWriteMethod(string method) =>
            method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
            method.Equals("PUT", StringComparison.OrdinalIgnoreCase) ||
            method.Equals("PATCH", StringComparison.OrdinalIgnoreCase) ||
            method.Equals("DELETE", StringComparison.OrdinalIgnoreCase);

        private static bool IsPublicPath(string path) =>
            path.StartsWith("/swagger") ||
            path.StartsWith("/api/auth") ||
            path.StartsWith("/api/billing/webhook") ||
            path.StartsWith("/api/industries") ||
            path.StartsWith("/api/fleetcategories") ||
            path.StartsWith("/api/equipmenttypes");

        private static bool IsBillingPath(string path) =>
            path.StartsWith("/api/billing");

        private static bool IsOnboardingAllowedPath(string path) =>
            // allow billing + your onboarding endpoints
            path.StartsWith("/api/billing") ||
            path.StartsWith("/api/onboarding") ||
            // optionally allow read-only basics
            path.StartsWith("/api/industries") ||
            path.StartsWith("/api/fleetcategories") ||
            path.StartsWith("/api/equipmenttypes");
    }
}
