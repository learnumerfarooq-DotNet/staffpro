// ─────────────────────────────────────────────────────────────────────────
// DomainEvent.cs — Base class for ALL domain events
//
// IN WEEK 1 (Day 3), DomainEvent was defined as a stub INSIDE
// CompanyCreatedEvent.cs. This worked but was messy — a base class
// embedded in a file named after a specific event.
//
// TODAY: We give DomainEvent its own proper file in the Common folder.
// The stub inside CompanyCreatedEvent.cs gets REMOVED.
//
// WHAT IS A DOMAIN EVENT?
//   A record that something important happened in the domain.
//   Examples:
//     CompanyCreatedEvent    → triggers welcome email in NotificationService
//     CompanyUpdatedEvent    → triggers audit log in AuditService
//     SetupCompletedEvent    → triggers billing activation in BillingService
//
// WHY NOT JUST CALL OTHER SERVICES DIRECTLY?
//   Direct call:  Company.Create() → calls NotificationService.SendEmail()
//     Problem: If NotificationService is down → Company creation FAILS!
//   Domain event: Company.Create() → raises CompanyCreatedEvent
//     Benefit: Company is saved regardless of notification status ✅
// ─────────────────────────────────────────────────────────────────────────

namespace CompanyService.Domain.Common;

/// <summary>
/// Base class for all domain events in StaffPro.
/// Every specific event (CompanyCreatedEvent, etc.) inherits from this.
/// </summary>
public abstract class DomainEvent
{
    /// <summary>
    /// Unique ID for this specific event occurrence.
    /// Used for deduplication — prevents processing the same event twice.
    /// </summary>
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Exact UTC moment this event occurred.
    /// Automatically set — event code never needs to provide this.
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <summary>
    /// The class name of the event.
    /// Example: "CompanyCreatedEvent", "CompanyUpdatedEvent"
    /// Used for routing events to the correct handler/queue.
    /// </summary>
    public string EventType => GetType().Name;
}