using System.Security.Claims;

namespace FleetManage.Api.Interfaces
{
    public interface ITenantContext
    {
        Guid TenantId { get; }
        string? UserId { get; }
    }

    /// <summary>
    /// Reads tenant/user from the authenticated JWT claims on the current HTTP request.
    /// IMPORTANT: We fail fast when the tenantId claim is missing/invalid to prevent
    /// creating rows with TenantId = Guid.Empty (which later "disappear" due to tenant filters).
    /// </summary>
    public sealed class HttpTenantContext : ITenantContext
    {
        private readonly IHttpContextAccessor _http;

        public HttpTenantContext(IHttpContextAccessor http) => _http = http;

        public Guid TenantId
        {
            get
            {
                var ctx = _http.HttpContext;

                // No request context available (background tasks / startup / etc.)
                if (ctx is null)
                    return Guid.Empty;

                // If request is not authenticated, we don't have a tenant.
                // Controllers that require tenant should be [Authorize] anyway.
                if (ctx.User?.Identity?.IsAuthenticated != true)
                    return Guid.Empty;

                var raw = ctx.User.FindFirst("tenantId")?.Value;

                // FAIL FAST: missing or invalid tenant claim should never be allowed,
                // otherwise tenant injection won't run and data will "vanish" later.
                if (!Guid.TryParse(raw, out var tenantId) || tenantId == Guid.Empty)
                    throw new UnauthorizedAccessException("Missing or invalid 'tenantId' claim in JWT.");

                return tenantId;
            }
        }

        public string? UserId =>
            _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
