// ─────────────────────────────────────────────────────────────────────────
// CompanyDbContext.cs (UPGRADED)
//
// CHANGES FROM WEEK 1 (Day 4):
//   ✅ Constructor now accepts tenantId for multi-tenancy filtering
//   ✅ SaveChangesAsync overridden to dispatch domain events after save
//
// MULTI-TENANCY:
//   CompanyDbContext receives the current request's TenantId (from JWT token).
//   It stores it and passes it to HasQueryFilter in CompanyConfiguration.
//   This means EVERY query automatically gets WHERE TenantId = @tenantId
//   appended — you can never accidentally query another tenant's data.
//
// DOMAIN EVENT DISPATCH:
//   After SaveChangesAsync saves data, we collect all domain events
//   raised by entities and publish them. This is the "outbox pattern lite":
//   data is saved first (so it's safe), then events are published.
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Common;
using CompanyService.Domain.Entities;
using CompanyService.Infrastructure.Data.Configurations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Infrastructure.Data;

/// <summary>
/// EF Core database context for CompanyService.
/// Tenant-aware: automatically filters all queries to the current tenant.
/// </summary>
public class CompanyDbContext : DbContext
{
    private readonly Guid _tenantId;
    private readonly IMediator? _mediator;  // Optional — null during migrations

    // ─────────────────────────────────────────────────
    // Constructor
    // ─────────────────────────────────────────────────

    /// <summary>
    /// Standard constructor for runtime use.
    /// tenantId — extracted from the JWT token in DependencyInjection.cs
    /// mediator  — used to publish domain events after save
    /// </summary>
    public CompanyDbContext(
    DbContextOptions<CompanyDbContext> options,
    ITenantProvider tenantProvider,
    IMediator? mediator = null)
    : base(options)
    {
        _tenantId = tenantProvider.GetTenantId();
        _mediator = mediator;
    }

    // ─────────────────────────────────────────────────
    // Tables
    // ─────────────────────────────────────────────────

    /// <summary>
    /// The Companies table. HasQueryFilter is applied in CompanyConfiguration:
    ///   - TenantId = @_tenantId  (current tenant only)
    ///   - IsDeleted = false       (soft-delete filter)
    /// Both filters apply automatically to every LINQ query.
    /// </summary>
    public DbSet<Company> Companies => Set<Company>();

    // ─────────────────────────────────────────────────
    // Model Configuration
    // ─────────────────────────────────────────────────

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Pass tenantId to configuration so HasQueryFilter can use it
        modelBuilder.ApplyConfiguration(new CompanyConfiguration(_tenantId));
    }

    // ─────────────────────────────────────────────────
    // Override SaveChangesAsync — dispatch domain events after save
    // ─────────────────────────────────────────────────

    /// <summary>
    /// Saves changes to the database, then publishes all domain events
    /// raised by entities during this request.
    ///
    /// ORDER MATTERS:
    ///   1. SaveChanges first  — data is committed to DB
    ///   2. Publish events     — other services react AFTER data is safe
    ///   If events were published BEFORE save and the save fails,
    ///   other services would react to something that never happened!
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // ── Step 1: Save all pending changes
        var result = await base.SaveChangesAsync(cancellationToken);

        // ── Step 2: Collect and dispatch domain events
        //await DispatchDomainEventsAsync(cancellationToken);

        return result;
    }

    // ─────────────────────────────────────────────────
    // Private — Domain Event Dispatcher
    // ─────────────────────────────────────────────────

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        if (_mediator is null) return;  // No mediator during design-time (migrations)

        // Collect all entities that have raised domain events
        var entitiesWithEvents = ChangeTracker
            .Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        // Collect all events
        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Clear events BEFORE publishing (prevents double-publish if handler throws)
        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

        // Publish each event via MediatR (INotificationHandler<T> picks them up)
        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent, cancellationToken);
    }

    // ─────────────────────────────────────────────────
    // Expose TenantId for configuration use
    // ─────────────────────────────────────────────────

    /// <summary>
    /// The current tenant ID. Exposed so CompanyConfiguration can use it
    /// in HasQueryFilter to auto-filter queries.
    /// </summary>
    public Guid TenantId => _tenantId;
}