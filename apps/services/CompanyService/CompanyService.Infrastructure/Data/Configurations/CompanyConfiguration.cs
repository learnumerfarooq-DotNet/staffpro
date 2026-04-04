// ─────────────────────────────────────────────────────────────────────────
// CompanyConfiguration.cs — EF Core Table Mapping
//
// This tells EF Core exactly how to map the Company entity to SQL Server:
//   - Column names, types, constraints
//   - Indexes for performance
//   - How to store Value Objects (Address as owned type)
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core Fluent API configuration for the Company entity.
/// Implements IEntityTypeConfiguration to keep mappings organized.
/// </summary>
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        // ── Table name
        builder.ToTable("Companies");

        // ── Primary Key
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("Id")
            .IsRequired();

        // ── Company Name
        builder.Property(c => c.Name)
            .HasColumnName("Name")
            .HasMaxLength(200)
            .IsRequired();

        // ── Trade Name
        builder.Property(c => c.TradeName)
            .HasColumnName("TradeName")
            .HasMaxLength(200)
            .IsRequired();

        // ── Industry
        builder.Property(c => c.Industry)
            .HasColumnName("Industry")
            .HasMaxLength(100)
            .IsRequired();

        // ── Size (stored as int in DB, displayed as enum in C#)
        builder.Property(c => c.Size)
            .HasColumnName("Size")
            .IsRequired()
            .HasConversion<int>();

        // ── Contact fields
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

        // ── Tax Number — UNIQUE constraint
        builder.Property(c => c.TaxNumber)
            .HasColumnName("TaxNumber")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(c => c.TaxNumber)
            .IsUnique()
            .HasDatabaseName("IX_Companies_TaxNumber");

        // ── Status fields
        builder.Property(c => c.IsSetupComplete)
            .HasColumnName("IsSetupComplete")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.IsActive)
            .HasColumnName("IsActive")
            .IsRequired()
            .HasDefaultValue(true);

        // ── Timestamps
        builder.Property(c => c.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .IsRequired()
            .HasColumnType("datetime2");

        // ── Address (Value Object stored as OWNED ENTITY — same table, no FK)
        builder.OwnsOne(c => c.HeadOfficeAddress, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Address_Street")
                .HasMaxLength(300)
                .IsRequired();

            address.Property(a => a.ApartmentSuite)
                .HasColumnName("Address_ApartmentSuite")
                .HasMaxLength(100)
                .IsRequired(false);

            address.Property(a => a.City)
                .HasColumnName("Address_City")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.State)
                .HasColumnName("Address_State")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.PostalCode)
                .HasColumnName("Address_PostalCode")
                .HasMaxLength(20)
                .IsRequired();

            address.Property(a => a.Country)
                .HasColumnName("Address_Country")
                .HasMaxLength(100)
                .IsRequired();
        });

        // ── Ignore domain events (not persisted in DB)
        builder.Ignore(c => c.DomainEvents);

        // ── Performance index on IsActive (most queries filter by active companies)
        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("IX_Companies_IsActive");
    }
}