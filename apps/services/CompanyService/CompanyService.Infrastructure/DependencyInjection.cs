// ─────────────────────────────────────────────────────────────────────────
// DependencyInjection.cs (UPDATED)
//
// CHANGE FROM WEEK 1 (Day 4):
//   CompanyDbContext now requires a Guid tenantId parameter.
//   For now we use a placeholder Guid — in Week 4 (JWT auth) this will be
//   replaced with the actual tenant ID from the JWT token claims.
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Interfaces;
using CompanyService.Infrastructure.Data;
using CompanyService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Register EF Core with SQL Server
        services.AddDbContext<CompanyDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(CompanyDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
        });

        // ── Register CompanyDbContext factory with tenantId
        //    Week 4 will replace this placeholder with JWT token extraction:
        //    Guid.Parse(httpContext.User.FindFirst("tenantId").Value)
        services.AddDbContext<CompanyDbContext>((serviceProvider, options) =>
        {
            // 1. Configure SQL Server
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            // 2. Resolve tenant provider (optional, just for clarity)
            var tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();

            // CompanyDbContext constructor takes ITenantProvider and options
            // Mediator is optional; will be injected at runtime if needed
        });

        // ── Register Repositories
        services.AddScoped<ICompanyRepository, CompanyRepository>();

        return services;
    }
}