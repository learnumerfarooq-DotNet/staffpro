// ─────────────────────────────────────────────────────────────────────────
// UpdateCompanyCommand.cs — CQRS Command for updating a company
//
// This was an empty scaffold from Week 1 Day 3. Completing it today.
//
// WHAT IT UPDATES:
//   - Company details (name, industry, size)
//   - Head office address
//   - Contact info (email, phone, website)
//
// WHAT IT DOES NOT UPDATE:
//   - TaxNumber — immutable after registration (legal requirement)
//   - Status — managed via separate commands (Archive, Reactivate)
//   - TenantId — immutable, set at creation
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Application.DTOs;
using MediatR;

namespace CompanyService.Application.Commands.UpdateCompany;

/// <summary>Command to update an existing company's information.</summary>
public record UpdateCompanyCommand(
    Guid CompanyId,
    string Name,
    string TradeName,
    string Industry,
    string Size,
    UpdateCompanyAddressCommand HeadOfficeAddress,
    string ContactEmail,
    string ContactPhone,
    string? Website
) : IRequest<CompanyDto>;

/// <summary>Nested address data for the update command.</summary>
public record UpdateCompanyAddressCommand(
    string Street,
    string? ApartmentSuite,
    string City,
    string State,
    string PostalCode,
    string Country
);