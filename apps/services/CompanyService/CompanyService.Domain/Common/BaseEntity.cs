// ─────────────────────────────────────────────────────────────────────────
// BaseEntity.cs — Abstract base class for ALL domain entities
//
// WHY THIS EXISTS:
//   In Week 1, Company.cs had its own Id, CreatedAt, UpdatedAt, and
//   a private _domainEvents list hard-coded inside it.
//   When we build Employee, Client, Job — we would copy-paste the same
//   properties into every entity. That is a maintenance nightmare.
//
//   BaseEntity solves this:
//     Company    : BaseEntity → gets Id, TenantId, CreatedAt, UpdatedAt, events ✅
//     Employee   : BaseEntity → same, automatically ✅
//     Department : BaseEntity → same, automatically ✅
//
// MULTI-TENANCY (TenantId):
//   StaffPro is a SAAS app — many companies (tenants) share the same database.
//   TenantId = "which company does this record belong to?"
//   Every entity carries TenantId so we can filter by it in EF Core.
//
// DOMAIN EVENTS:
//   An entity "raises" events when important things happen.
//   The Infrastructure layer collects and publishes them after SaveChanges().
//   BaseEntity owns the _domainEvents list so every entity can do this.
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Events;

namespace CompanyService.Domain.Common;

/// <summary>
/// Abstract base class inherited by every domain entity in StaffPro.
/// Provides: identity (Id), multi-tenancy (TenantId), audit timestamps,
/// and domain event infrastructure.
/// </summary>
public abstract class BaseEntity
{
    // ─────────────────────────────────────────────────
    // Domain Events — private so only THIS class manages the list
    // ─────────────────────────────────────────────────

    private readonly List<DomainEvent> _domainEvents = [];

    /// <summary>
    /// Read-only view of domain events raised by this entity.
    /// External code can READ but not MODIFY the list.
    /// </summary>
    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // ─────────────────────────────────────────────────
    // Identity
    // ─────────────────────────────────────────────────

    /// <summary>
    /// Globally unique identifier for this entity.
    /// Using Guid (not int) because:
    ///   - Generated in C# before hitting the database
    ///   - Safe to use across distributed microservices
    ///   - Cannot be guessed or enumerated by attackers
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Which tenant (company) this record belongs to.
    /// In a SAAS system, TechCorp and MedCorp share the same DB.
    /// TenantId ensures each company sees only its own data.
    /// Set once at creation — never changes.
    /// </summary>
    public Guid TenantId { get; protected set; }

    // ─────────────────────────────────────────────────
    // Audit Timestamps
    // ─────────────────────────────────────────────────

    /// <summary>
    /// UTC timestamp when this entity was first created.
    /// Always UTC — never local time — so the app works globally.
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// UTC timestamp of the most recent update.
    /// Nullable: null means "never updated since creation".
    /// Set automatically by entity methods when state changes.
    /// </summary>
    public DateTime? UpdatedAt { get; protected set; }

    // ─────────────────────────────────────────────────
    // Constructor
    // ─────────────────────────────────────────────────

    /// <summary>
    /// Protected constructor — subclasses call this via base(tenantId).
    /// Automatically generates a new Id and records creation time.
    /// </summary>
    protected BaseEntity(Guid tenantId)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        CreatedAt = DateTime.UtcNow;
    }

    // ─────────────────────────────────────────────────
    // Domain Event Helpers
    // ─────────────────────────────────────────────────

    /// <summary>
    /// Called by entity methods (e.g., Create, CompleteSetup) to record
    /// that something important happened. Events are published AFTER
    /// the entity is saved to the database.
    /// </summary>
    protected void RaiseEvent(DomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Called by Infrastructure after events have been published.
    /// Prevents the same event being published twice.
    /// </summary>
    public void ClearDomainEvents()
        => _domainEvents.Clear();
}