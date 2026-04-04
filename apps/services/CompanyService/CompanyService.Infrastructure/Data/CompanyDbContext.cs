// ─────────────────────────────────────────────────────────────────────────
// CompanyDbContext.cs — EF Core Database Context
//
// DbContext is the bridge between your C# entities and the database.
// It tracks changes and generates SQL queries automatically.
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Entities;
using CompanyService.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for the CompanyService.
/// Manages the Companies table and all database operations.
/// </summary>
public class CompanyDbContext : DbContext
{
    public CompanyDbContext(DbContextOptions<CompanyDbContext> options) : base(options) { }

    // ─────────────────────────────────────────────────
    // Tables
    // ─────────────────────────────────────────────────

    /// <summary>The Companies table</summary>
    public DbSet<Company> Companies => Set<Company>();

    // ─────────────────────────────────────────────────
    // Model Configuration
    // ─────────────────────────────────────────────────

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity type configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompanyDbContext).Assembly);
    }
}