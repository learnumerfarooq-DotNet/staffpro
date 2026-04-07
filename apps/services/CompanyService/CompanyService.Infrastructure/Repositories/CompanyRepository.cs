// ─────────────────────────────────────────────────────────────────────────
// CompanyRepository.cs (UPGRADED)
//
// CHANGES FROM WEEK 1 (Day 4):
//   ✅ Added AsNoTracking() to all READ queries
//      WHY: AsNoTracking skips EF change-tracking for read-only queries.
//           30–50% faster, uses less memory. Use it whenever you will NOT
//           call UpdateAsync on the returned entity in the same request.
//           For UPDATE flows: load WITHOUT AsNoTracking → track → save.
//   ✅ Added GetByTenantIdAsync
//   ✅ Added GetAllAsync with pagination
//   ✅ Added ExistsByIdAsync
//   ✅ Added DeleteAsync (soft delete via entity method)
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Entities;
using CompanyService.Domain.Interfaces;
using CompanyService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly CompanyDbContext _context;

    public CompanyRepository(CompanyDbContext context)
        => _context = context;

    // ─────────────────────────────────
    // Read Operations
    // ─────────────────────────────────

    /// <summary>
    /// Get by ID — uses AsNoTracking (read-only, no update needed).
    /// HasQueryFilter auto-adds: WHERE TenantId = @tenantId AND IsDeleted = 0
    /// </summary>
    public async Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    /// <summary>
    /// Get the company for a specific tenant.
    /// In StaffPro, one tenant = one company, so this returns a single record.
    /// </summary>
    public async Task<Company?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
        => await _context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.TenantId == tenantId, cancellationToken);

    public async Task<Company?> GetByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken = default)
        => await _context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.TaxNumber == taxNumber, cancellationToken);

    /// <summary>
    /// Paginated list — bypasses tenant filter via IgnoreQueryFilters().
    /// Returns both the page of data AND the total record count.
    /// Total count is needed by Angular's paginator to compute total pages.
    /// </summary>
    public async Task<(IReadOnlyList<Company> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        // IgnoreQueryFilters = bypass HasQueryFilter (admin use — all tenants)
        var query = _context.Companies
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(c => !c.IsDeleted)   // Still hide soft-deleted records
            .OrderBy(c => c.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Companies
            .AnyAsync(c => c.Id == id, cancellationToken);

    public async Task<bool> ExistsByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken = default)
        => await _context.Companies
            .AnyAsync(c => c.TaxNumber == taxNumber, cancellationToken);

    // ─────────────────────────────────
    // Write Operations
    // ─────────────────────────────────

    public async Task AddAsync(Company company, CancellationToken cancellationToken = default)
        => await _context.Companies.AddAsync(company, cancellationToken);

    /// <summary>
    /// For UPDATE: do NOT use AsNoTracking — EF needs to track the entity
    /// to detect which properties changed and generate the UPDATE SQL.
    /// </summary>
    public Task UpdateAsync(Company company, CancellationToken cancellationToken = default)
    {
        _context.Companies.Update(company);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Soft delete: load → call entity.Delete() → update.
    /// Calls company.Delete() so business rules are enforced and
    /// CompanyDeletedEvent is raised before we save.
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Load WITHOUT AsNoTracking so EF tracks the change
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (company is null) return;  // Already gone — nothing to do

        company.Delete();              // Sets IsDeleted = true via business method
        _context.Companies.Update(company);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}