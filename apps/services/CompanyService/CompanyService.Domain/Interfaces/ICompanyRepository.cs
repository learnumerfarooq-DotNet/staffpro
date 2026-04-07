// ─────────────────────────────────────────────────────────────────────────
// ICompanyRepository.cs (UPGRADED)
//
// CHANGES FROM WEEK 1 (Day 3):
//   ✅ Added GetByTenantIdAsync — primary lookup for tenant's own company
//   ✅ Added DeleteAsync       — soft delete using company.Delete()
//   ✅ Added GetAllAsync       — paginated list for admin
//   ✅ Added ExistsByIdAsync   — check existence by GUID
//
// DESIGN NOTE — AsNoTracking:
//   The interface does NOT specify AsNoTracking. That is an implementation
//   detail of the repository. The interface only defines WHAT, not HOW.
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Entities;

namespace CompanyService.Domain.Interfaces;

/// <summary>
/// Contract for all Company persistence operations.
/// Domain defines this. Infrastructure implements it.
/// </summary>
public interface ICompanyRepository
{
    // ─────────────────────────────────
    // Read Operations
    // ─────────────────────────────────

    /// <summary>
    /// Get a company by its unique GUID.
    /// Returns null if not found or soft-deleted.
    /// </summary>
    Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the company that belongs to a specific tenant.
    /// In StaffPro, each tenant registers exactly ONE company.
    /// This is the most common lookup — called on every authenticated request.
    /// Returns null if the tenant has not yet registered their company.
    /// </summary>
    Task<Company?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a company by its unique tax registration number.
    /// Used to enforce the duplicate-tax-number business rule.
    /// </summary>
    Task<Company?> GetByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a paginated list of all companies (admin use — bypasses tenant filter).
    /// pageNumber starts at 1. pageSize is the max number of records per page.
    /// Returns (items, totalCount) so the caller can compute total pages.
    /// </summary>
    Task<(IReadOnlyList<Company> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>Check if a company with the given ID exists (not soft-deleted).</summary>
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Check if a company with the given tax number already exists.</summary>
    Task<bool> ExistsByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken = default);

    // ─────────────────────────────────
    // Write Operations
    // ─────────────────────────────────

    /// <summary>Add a new company to the repository (not saved until SaveChangesAsync).</summary>
    Task AddAsync(Company company, CancellationToken cancellationToken = default);

    /// <summary>Mark an existing company for update (not saved until SaveChangesAsync).</summary>
    Task UpdateAsync(Company company, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-delete a company by ID.
    /// Internally calls company.Delete() which sets IsDeleted = true.
    /// Not saved until SaveChangesAsync is called.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Commit all pending changes to the database.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}