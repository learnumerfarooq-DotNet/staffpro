// ─────────────────────────────────────────────────────────────────────────
// CompanyCreatedEvent.cs — Domain Event
//
// Domain Events notify other parts of the system that something important
// happened. For example, when a Company is created, we might want to:
//   - Send a welcome email (NotificationService listens to this)
//   - Create default settings
//   - Log the creation for audit
// ─────────────────────────────────────────────────────────────────────────

namespace CompanyService.Domain.Events;

/// <summary>
/// Base class for all domain events.
/// </summary>
public abstract class DomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => GetType().Name;
}

/// <summary>
/// Raised when a new Company is created in the system.
/// Consumed by: NotificationService (welcome email), AuditService
/// </summary>
public sealed class CompanyCreatedEvent : DomainEvent
{
    public Guid CompanyId { get; }
    public string CompanyName { get; }
    public string ContactEmail { get; }

    public CompanyCreatedEvent(Guid companyId, string companyName, string contactEmail)
    {
        CompanyId = companyId;
        CompanyName = companyName;
        ContactEmail = contactEmail;
    }
}

/// <summary>
/// Raised when company details are updated.
/// </summary>
public sealed class CompanyUpdatedEvent : DomainEvent
{
    public Guid CompanyId { get; }
    public string CompanyName { get; }

    public CompanyUpdatedEvent(Guid companyId, string companyName)
    {
        CompanyId = companyId;
        CompanyName = companyName;
    }
}

/// <summary>
/// Raised when company onboarding setup is completed.
/// </summary>
public sealed class CompanySetupCompletedEvent : DomainEvent
{
    public Guid CompanyId { get; }

    public CompanySetupCompletedEvent(Guid companyId)
    {
        CompanyId = companyId;
    }
}