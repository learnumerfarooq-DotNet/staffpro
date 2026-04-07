// ─────────────────────────────────────────────────────────────────────────
// Company.cs — Company Aggregate Root (UPGRADED with Status + IsDeleted)
//
// CHANGES FROM DAY 8:
//   ✅ Replaced bool IsActive + bool IsSetupComplete with CompanyStatus enum
//   ✅ Added bool IsDeleted for soft delete
//   ✅ Added proper lifecycle transitions: Activate(), Archive(), Delete()
//   ✅ All lifecycle methods enforce valid state transitions
//   ✅ New domain events: CompanyArchivedEvent, CompanyDeletedEvent
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Common;
using CompanyService.Domain.Events;
using CompanyService.Domain.Exceptions;
using CompanyService.Domain.ValueObjects;

namespace CompanyService.Domain.Entities;

/// <summary>
/// Company aggregate root. Tracks status through the full lifecycle:
/// Pending → Active → Archived (and soft-deletable at any point).
/// </summary>
public sealed class Company : BaseEntity
{
    // ─────────────────────────────────────────────────
    // Properties
    // ─────────────────────────────────────────────────

    public string Name { get; private set; } = string.Empty;
    public string TradeName { get; private set; } = string.Empty;
    public string Industry { get; private set; } = string.Empty;
    public CompanySize Size { get; private set; }
    public Address HeadOfficeAddress { get; private set; } = null!;
    public string ContactEmail { get; private set; } = string.Empty;
    public string ContactPhone { get; private set; } = string.Empty;
    public string? Website { get; private set; }
    public string TaxNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Replaces the Week 1 bool IsActive + bool IsSetupComplete pair.
    /// One enum carries both pieces of information:
    ///   Pending  = registered, wizard incomplete (was: IsActive=true, IsSetupComplete=false)
    ///   Active   = fully operational             (was: IsActive=true, IsSetupComplete=true)
    ///   Archived = suspended/closed              (was: IsActive=false)
    /// </summary>
    public CompanyStatus Status { get; private set; }

    /// <summary>
    /// Soft-delete flag.
    /// true  = logically deleted (hidden from all normal queries via EF QueryFilter)
    /// false = not deleted (default)
    ///
    /// WHY SOFT DELETE INSTEAD OF HARD DELETE?
    ///   Hard delete: DELETE FROM Companies WHERE Id = ...
    ///     Problem: Breaks audit logs, foreign keys, and any archived reports
    ///   Soft delete: UPDATE Companies SET IsDeleted = 1
    ///     Benefit: Data preserved for legal/audit; can be undeleted by admin
    /// </summary>
    public bool IsDeleted { get; private set; }

    // ─────────────────────────────────────────────────
    // Constructor
    // ─────────────────────────────────────────────────

    private Company(Guid tenantId) : base(tenantId) { }

    // ─────────────────────────────────────────────────
    // FACTORY METHOD
    // ─────────────────────────────────────────────────

    public static Company Create(
        Guid tenantId,
        string name,
        string tradeName,
        string industry,
        CompanySize size,
        Address headOfficeAddress,
        string contactEmail,
        string contactPhone,
        string taxNumber,
        string? website = null)
    {
        if (tenantId == Guid.Empty)
            throw new DomainException("TenantId cannot be empty.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Company name is required.");
        if (name.Length > 200)
            throw new DomainException("Company name cannot exceed 200 characters.");
        if (!IsValidEmail(contactEmail))
            throw new DomainException($"Contact email '{contactEmail}' is not valid.");
        if (string.IsNullOrWhiteSpace(taxNumber))
            throw new DomainException("Tax number is required.");

        var company = new Company(tenantId)
        {
            Name = name.Trim(),
            TradeName = tradeName.Trim(),
            Industry = industry.Trim(),
            Size = size,
            HeadOfficeAddress = headOfficeAddress,
            ContactEmail = contactEmail.Trim().ToLowerInvariant(),
            ContactPhone = contactPhone.Trim(),
            TaxNumber = taxNumber.Trim(),
            Website = website?.Trim(),
            Status = CompanyStatus.Pending,   // ← always starts Pending
            IsDeleted = false
        };

        company.RaiseEvent(new CompanyCreatedEvent(company.Id, company.Name, company.ContactEmail));
        return company;
    }

    // ─────────────────────────────────────────────────
    // LIFECYCLE TRANSITIONS
    // ─────────────────────────────────────────────────

    /// <summary>
    /// Complete the onboarding wizard → moves Pending to Active.
    ///
    /// Business Rules:
    ///   - Can only be called when Status is Pending
    ///   - Deleted companies cannot be activated
    /// </summary>
    public void CompleteSetup()
    {
        if (IsDeleted)
            throw new DomainException("Cannot activate a deleted company.");
        if (Status != CompanyStatus.Pending)
            throw new DomainException(
                $"Cannot complete setup. Company status is '{Status}'. Expected 'Pending'.");

        Status = CompanyStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        RaiseEvent(new CompanySetupCompletedEvent(Id));
    }

    /// <summary>
    /// Archive (suspend) the company → moves Active to Archived.
    ///
    /// Business Rules:
    ///   - Can only be called when Status is Active
    ///   - Deleted companies cannot be archived
    /// </summary>
    public void Archive()
    {
        if (IsDeleted)
            throw new DomainException("Cannot archive a deleted company.");
        if (Status == CompanyStatus.Archived)
            throw new DomainException("Company is already archived.");
        if (Status == CompanyStatus.Pending)
            throw new DomainException("Cannot archive a company that has not completed setup.");

        Status = CompanyStatus.Archived;
        UpdatedAt = DateTime.UtcNow;

        RaiseEvent(new CompanyArchivedEvent(Id));
    }

    /// <summary>
    /// Reactivate an archived company → moves Archived back to Active.
    ///
    /// Business Rules:
    ///   - Can only be called when Status is Archived
    ///   - Deleted companies cannot be reactivated
    /// </summary>
    public void Reactivate()
    {
        if (IsDeleted)
            throw new DomainException("Cannot reactivate a deleted company.");
        if (Status != CompanyStatus.Archived)
            throw new DomainException(
                $"Cannot reactivate. Company status is '{Status}'. Expected 'Archived'.");

        Status = CompanyStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft-delete the company. Sets IsDeleted = true.
    /// The company is hidden from all normal queries via EF Query Filter.
    ///
    /// Business Rules:
    ///   - Cannot delete an already-deleted company
    /// </summary>
    public void Delete()
    {
        if (IsDeleted)
            throw new DomainException("Company is already deleted.");

        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;

        RaiseEvent(new CompanyDeletedEvent(Id));
    }

    // ─────────────────────────────────────────────────
    // UPDATE METHODS
    // ─────────────────────────────────────────────────

    public void UpdateDetails(string name, string tradeName, string industry, CompanySize size)
    {
        if (IsDeleted)
            throw new DomainException("Cannot update a deleted company.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Company name cannot be empty.");

        Name = name.Trim();
        TradeName = tradeName.Trim();
        Industry = industry.Trim();
        Size = size;
        UpdatedAt = DateTime.UtcNow;

        RaiseEvent(new CompanyUpdatedEvent(Id, Name));
    }

    public void UpdateHeadOffice(Address newAddress)
    {
        if (IsDeleted)
            throw new DomainException("Cannot update a deleted company.");

        HeadOfficeAddress = newAddress ?? throw new DomainException("Address cannot be null.");
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateContactInfo(string contactEmail, string contactPhone, string? website)
    {
        if (IsDeleted)
            throw new DomainException("Cannot update a deleted company.");
        if (!IsValidEmail(contactEmail))
            throw new DomainException($"Contact email '{contactEmail}' is not valid.");

        ContactEmail = contactEmail.Trim().ToLowerInvariant();
        ContactPhone = contactPhone.Trim();
        Website = website?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    // ─────────────────────────────────────────────────
    // Private Helpers
    // ─────────────────────────────────────────────────

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email.Trim();
        }
        catch { return false; }
    }
}

/// <summary>Company size categories.</summary>
public enum CompanySize
{
    Startup = 1,
    Small = 2,
    Medium = 3,
    Large = 4,
    Enterprise = 5
}