namespace CompanyService.Infrastructure.Data;

public interface ITenantProvider
{
    Guid GetTenantId();
}