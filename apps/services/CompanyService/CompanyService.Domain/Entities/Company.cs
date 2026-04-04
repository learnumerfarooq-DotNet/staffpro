// ─────────────────────────────────────────────────────────────────────────
// Company.cs — The Company Aggregate Root
//
// An "Aggregate Root" is the main entity that all others in a domain
// belong to. Rules:
//   1. Only this class can mutate its own state
//   2. All business rules are INSIDE this class
//   3. No direct DB calls — just pure C# logic
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Events;
using CompanyService.Domain.Exceptions;
using CompanyService.Domain.ValueObjects;
using System.Net;

namespace CompanyService.Domain.Entities;

/// <summary>
/// The core Company entity. Represents an organization using StaffPro.
/// This is the Aggregate Root for the Company bounded context.
/// </summary>
public sealed class Company
{
    // ─────────────────────────────────────────────────
    // Private backing fields (no direct external access)
    // ─────────────────────────────────────────────────

    private readonly List<DomainEvent> _domainEvents = [];

    // ─────────────────────────────────────────────────
    // Properties (read-only externally, set by this class only)
    // ─────────────────────────────────────────────────

    /// <summary>Unique identifier (GUID)</summary>
    public Guid Id { get; private set; }

    /// <summary>Legal business name of the company</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Short display name / brand name</summary>
    public string TradeName { get; private set; } = string.Empty;

    /// <summary>Industry the company operates in</summary>
    public string Industry { get; private set; } = string.Empty;

    /// <summary>Company size category</summary>
    public CompanySize Size { get; private set; }

    /// <summary>Primary address of the company</summary>
    public Address HeadOfficeAddress { get; private set; } = null!;

    /// <summary>Primary contact email</summary>
    public string ContactEmail { get; private set; } = string.Empty;

    /// <summary>Primary contact phone number</summary>
    public string ContactPhone { get; private set; } = string.Empty;

    /// <summary>Company website URL</summary>
    public string? Website { get; private set; }

    /// <summary>Tax registration number</summary>
    public string TaxNumber { get; private set; } = string.Empty;

    /// <summary>Whether the company has completed the onboarding wizard</summary>
    public bool IsSetupComplete { get; private set; }

    /// <summary>Whether the company account is active</summary>
    public bool IsActive { get; private set; }

    /// <summary>When this record was created (UTC)</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>When this record was last modified (UTC)</summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>Read-only domain events raised by this aggregate</summary>
    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // ─────────────────────────────────────────────────
    // Private constructor — use factory methods below
    // ─────────────────────────────────────────────────

    private Company() { }

    // ─────────────────────────────────────────────────
    // FACTORY METHOD — Create a new Company
    // ─────────────────────────────────────────────────

    /// <summary>
    /// Creates a new Company with all required business rules validated.
    /// Raises a CompanyCreatedEvent.
    /// </summary>
    public static Company Create(
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
        // ── Business Rule 1: Name is required
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Company name is required.");

        // ── Business Rule 2: Name cannot exceed 200 characters
        if (name.Length > 200)
            throw new DomainException("Company name cannot exceed 200 characters.");

        // ── Business Rule 3: Email must be valid
        if (!IsValidEmail(contactEmail))
            throw new DomainException($"Contact email '{contactEmail}' is not a valid email address.");

        // ── Business Rule 4: Tax number is required
        if (string.IsNullOrWhiteSpace(taxNumber))
            throw new DomainException("Tax number is required.");

        // ── Create the entity
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            TradeName = tradeName.Trim(),
            Industry = industry.Trim(),
            Size = size,
            HeadOfficeAddress = headOfficeAddress,
            ContactEmail = contactEmail.Trim().ToLowerInvariant(),
            ContactPhone = contactPhone.Trim(),
            TaxNumber = taxNumber.Trim(),
            Website = website?.Trim(),
            IsSetupComplete = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // ── Raise domain event (signals to other services that a company was created)
        company.RaiseEvent(new CompanyCreatedEvent(company.Id, company.Name, company.ContactEmail));

        return company;
    }

    // ─────────────────────────────────────────────────
    // METHODS — Business Operations
    // ─────────────────────────────────────────────────

    /// <summary>Update basic company details.</summary>
    public void UpdateDetails(string name, string tradeName, string industry, CompanySize size)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Company name cannot be empty.");

        Name = name.Trim();
        TradeName = tradeName.Trim();
        Industry = industry.Trim();
        Size = size;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Update the head office address.</summary>
    public void UpdateHeadOffice(Address newAddress)
    {
        HeadOfficeAddress = newAddress ?? throw new DomainException("Address cannot be null.");
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Mark company setup as completed (after finishing the wizard).</summary>
    public void CompleteSetup()
    {
        if (IsSetupComplete)
            throw new DomainException("Company setup is already complete.");

        IsSetupComplete = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Deactivate the company (soft delete).</summary>
    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Company is already deactivated.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Clear all domain events after they've been dispatched.</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    // ─────────────────────────────────────────────────
    // PRIVATE HELPERS
    // ─────────────────────────────────────────────────

    private void RaiseEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email.Trim();
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>Company size categories</summary>
public enum CompanySize
{
    Startup = 1,     // 1–10 employees
    Small = 2,       // 11–50 employees
    Medium = 3,      // 51–250 employees
    Large = 4,       // 251–1000 employees
    Enterprise = 5   // 1000+ employees
}