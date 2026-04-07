using CompanyService.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public TenantProvider(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public Guid GetTenantId()
    {
        var tenantClaim = _httpContextAccessor.HttpContext?
            .User?
            .FindFirst("tenantId")?.Value;

        if (!string.IsNullOrEmpty(tenantClaim))
            return Guid.Parse(tenantClaim);

        // ✅ fallback (DEV mode)
        var defaultTenant = _configuration["MultiTenancy:DefaultTenantId"];

        return Guid.Parse(defaultTenant!);
    }
}