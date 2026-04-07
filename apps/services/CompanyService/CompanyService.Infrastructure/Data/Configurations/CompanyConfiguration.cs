// ─────────────────────────────────────────────────────────────────────────
// CompanyConfiguration.cs (UPGRADED)
//
// CHANGES FROM WEEK 1 (Day 4):
//   ✅ Added Status column — stored as string ("Pending", "Active", "Archived")
//   ✅ Removed IsActive + IsSetupComplete columns (replaced by Status)
//   ✅ Added IsDeleted column — BIT NOT NULL DEFAULT 0
//   ✅ Added HasQueryFilter — auto-filters by TenantId AND IsDeleted = false
//   ✅ Added IX_Companies_TenantId index for query performance
//   ✅ Added IX_Companies_Status index for status-based queries
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Data.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    private readonly Guid _tenantId;

    /// <summary>
    /// Constructor receives tenantId so HasQueryFilter can close over it.
    /// Called from a custom ApplyConfiguration call in CompanyDbContext
    /// when the configuration needs tenant context.
    /// </summary>
    public CompanyConfiguration(Guid tenantId) => _tenantId = tenantId;

    public void Configure(EntityTypeBuilder<Company> builder)
    {
        // ── Table
        builder.ToTable("Companies");

        // ── Primary Key
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("Id")
            .IsRequired();

        // ── TenantId (from BaseEntity)
        builder.Property(c => c.TenantId)
            .HasColumnName("TenantId")
            .IsRequired();

        // ── Company Details
        builder.Property(c => c.Name)
            .HasColumnName("Name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.TradeName)
            .HasColumnName("TradeName")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Industry)
            .HasColumnName("Industry")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Size)
            .HasColumnName("Size")
            .IsRequired()
            .HasConversion<int>();   // Stored as 1,2,3,4,5 in DB

        // ── Status (stored as string — readable in SQL queries)
        //
        // HasConversion<string>() means:
        //   CompanyStatus.Pending  → "Pending"  stored in DB
        //   CompanyStatus.Active   → "Active"   stored in DB
        //   CompanyStatus.Archived → "Archived" stored in DB
        //
        // WHY STRING AND NOT INT?
        //   String: SELECT * FROM Companies WHERE Status = 'Active'  — readable!
        //   Int:    SELECT * FROM Companies WHERE Status = 2         — what is 2??
        builder.Property(c => c.Status)
            .HasColumnName("Status")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion<string>();

        // ── Contact Fields
        builder.Property(c => c.ContactEmail)
            .HasColumnName("ContactEmail")
            .HasMaxLength(254)
            .IsRequired();

        builder.Property(c => c.ContactPhone)
            .HasColumnName("ContactPhone")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.Website)
            .HasColumnName("Website")
            .HasMaxLength(2048)
            .IsRequired(false);

        // ── Tax Number — unique constraint
        builder.Property(c => c.TaxNumber)
            .HasColumnName("TaxNumber")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(c => c.TaxNumber)
            .IsUnique()
            .HasDatabaseName("IX_Companies_TaxNumber");

        // ── Soft Delete Flag (NEW — replaces IsActive bool)
        builder.Property(c => c.IsDeleted)
            .HasColumnName("IsDeleted")
            .IsRequired()
            .HasDefaultValue(false);

        // ── Audit Timestamps (from BaseEntity)
        builder.Property(c => c.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .IsRequired(false)          // NULL until first update
            .HasColumnType("datetime2");

        // ── Address Value Object — stored as owned columns in the same table
        builder.OwnsOne(c => c.HeadOfficeAddress, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Address_Street").HasMaxLength(300).IsRequired();
            address.Property(a => a.ApartmentSuite)
                .HasColumnName("Address_ApartmentSuite").HasMaxLength(100).IsRequired(false);
            address.Property(a => a.City)
                .HasColumnName("Address_City").HasMaxLength(100).IsRequired();
            address.Property(a => a.State)
                .HasColumnName("Address_State").HasMaxLength(100).IsRequired();
            address.Property(a => a.PostalCode)
                .HasColumnName("Address_PostalCode").HasMaxLength(20).IsRequired();
            address.Property(a => a.Country)
                .HasColumnName("Address_Country").HasMaxLength(100).IsRequired();
        });

        // ── Ignore domain events (not a database column — lives in memory only)
        builder.Ignore(c => c.DomainEvents);

        // ── MULTI-TENANT QUERY FILTER (the most important line in this file!)
        //
        // This single line appends TWO conditions to EVERY query:
        //   WHERE TenantId = @_tenantId    ← tenant isolation
        //   AND   IsDeleted = 0            ← soft-delete filter
        //
        // You NEVER need to add .Where(c => c.TenantId == tenantId) in queries.
        // EF Core adds it automatically. If you forget, EF still adds it.
        //
        // Example LINQ:
        //   var companies = await _context.Companies.ToListAsync();
        // Generated SQL:
        //   SELECT * FROM Companies WHERE TenantId = 'abc-123' AND IsDeleted = 0
        //
        // To bypass the filter (admin use only):
        //   _context.Companies.IgnoreQueryFilters().ToListAsync()
        builder.HasQueryFilter(c => c.TenantId == _tenantId && !c.IsDeleted);

        // ── Performance Indexes
        builder.HasIndex(c => c.TenantId)
            .HasDatabaseName("IX_Companies_TenantId");

        builder.HasIndex(c => new { c.TenantId, c.Status })
            .HasDatabaseName("IX_Companies_TenantId_Status");
    }
}