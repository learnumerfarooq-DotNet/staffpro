// ─────────────────────────────────────────────────────────────────────────
// CompanyRepository.cs — Repository Implementation
//
// Implements ICompanyRepository using Entity Framework Core.
// The Application layer only knows about the interface (ICompanyRepository)
// — it has no idea this implementation uses EF Core or SQL Server.
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
    {
        _context = context;
    }

    public async Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Company?> GetByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Companies
            .FirstOrDefaultAsync(c => c.TaxNumber == taxNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Company>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Companies
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Company company, CancellationToken cancellationToken = default)
    {
        await _context.Companies.AddAsync(company, cancellationToken);
    }

    public Task UpdateAsync(Company company, CancellationToken cancellationToken = default)
    {
        _context.Companies.Update(company);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Companies
            .AnyAsync(c => c.TaxNumber == taxNumber, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}