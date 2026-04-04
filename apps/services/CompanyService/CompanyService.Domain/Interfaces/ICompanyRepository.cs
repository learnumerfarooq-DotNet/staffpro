// ─────────────────────────────────────────────────────────────────────────
// ICompanyRepository.cs — Repository Interface (Contract)
//
// The Domain defines WHAT operations are needed (the interface).
// The Infrastructure layer implements HOW (with EF Core / SQL Server).
// This keeps Domain decoupled from any database technology.
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Entities;

namespace CompanyService.Domain.Interfaces;

/// <summary>
/// Contract for Company data access operations.
/// The Domain defines this. Infrastructure implements it.
/// </summary>
public interface ICompanyRepository
{
    /// <summary>Get a company by its unique ID</summary>
    Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Get a company by its tax number (must be unique)</summary>
    Task<Company?> GetByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken = default);

    /// <summary>Get all active companies (for admin purposes)</summary>
    Task<IReadOnlyList<Company>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>Add a new company to the repository</summary>
    Task AddAsync(Company company, CancellationToken cancellationToken = default);

    /// <summary>Update an existing company in the repository</summary>
    Task UpdateAsync(Company company, CancellationToken cancellationToken = default);

    /// <summary>Check if a company with the given tax number already exists</summary>
    Task<bool> ExistsByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken = default);

    /// <summary>Save all pending changes to the database</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}