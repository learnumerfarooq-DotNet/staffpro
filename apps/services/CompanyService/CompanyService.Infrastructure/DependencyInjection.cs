// ─────────────────────────────────────────────────────────────────────────
// DependencyInjection.cs — Infrastructure Service Registration
//
// This extension method registers all Infrastructure services with the
// ASP.NET Core DI container. The API project calls this at startup.
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
    /// <summary>
    /// Registers all Infrastructure layer services:
    /// - EF Core DbContext
    /// - Repository implementations
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Register EF Core with SQL Server
        services.AddDbContext<CompanyDbContext>(options =>
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

        // ── Register Repositories
        services.AddScoped<ICompanyRepository, CompanyRepository>();

        return services;
    }
}