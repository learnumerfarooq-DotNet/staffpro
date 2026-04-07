using CompanyService.Domain.Common;

namespace CompanyService.Domain.Events;

public sealed class CompanyCreatedEvent : DomainEvent
{
    public Guid CompanyId { get; }
    public string CompanyName { get; }
    public string ContactEmail { get; }
    public CompanyCreatedEvent(Guid companyId, string companyName, string contactEmail)
    { CompanyId = companyId; CompanyName = companyName; ContactEmail = contactEmail; }
}

public sealed class CompanyUpdatedEvent : DomainEvent
{
    public Guid CompanyId { get; }
    public string CompanyName { get; }
    public CompanyUpdatedEvent(Guid companyId, string companyName)
    { CompanyId = companyId; CompanyName = companyName; }
}

public sealed class CompanySetupCompletedEvent : DomainEvent
{
    public Guid CompanyId { get; }
    public CompanySetupCompletedEvent(Guid companyId) => CompanyId = companyId;
}

/// <summary>Raised when company is archived/suspended.</summary>
public sealed class CompanyArchivedEvent : DomainEvent
{
    public Guid CompanyId { get; }
    public CompanyArchivedEvent(Guid companyId) => CompanyId = companyId;
}

/// <summary>Raised when company is soft-deleted.</summary>
public sealed class CompanyDeletedEvent : DomainEvent
{
    public Guid CompanyId { get; }
    public CompanyDeletedEvent(Guid companyId) => CompanyId = companyId;
}