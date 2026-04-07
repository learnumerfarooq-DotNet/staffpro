using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace CompanyService.Infrastructure.Data;

public class CompanyDbContextFactory : IDesignTimeDbContextFactory<CompanyDbContext>
{
    public CompanyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CompanyDbContext>();

        // Use your connection string from appsettings.json
        optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=StaffPro_Company;User Id=sa;Password=StaffPro@Dev2024!;TrustServerCertificate=True;MultipleActiveResultSets=true"
        );

        // Use DesignTimeTenantProvider because JWT/HTTP is not available at design time
        return new CompanyDbContext(
            optionsBuilder.Options,
            new DesignTimeTenantProvider(),
            null
        );
    }
}

// Simple ITenantProvider for design-time usage
public class DesignTimeTenantProvider : ITenantProvider
{
    public Guid GetTenantId() => Guid.Empty; // Just a dummy tenant for migrations
}