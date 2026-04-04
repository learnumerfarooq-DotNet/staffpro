// ─────────────────────────────────────────────────────────────────────────
// CreateCompanyCommand.cs — CQRS Command
//
// A Command is a request to CHANGE something (POST/PUT/DELETE in REST).
// Commands use MediatR — you send a command → the handler executes it.
//
// Pattern:
//   Controller → sends CreateCompanyCommand
//   → MediatR routes to CreateCompanyCommandHandler
//   → Handler creates Company entity
//   → Handler saves via ICompanyRepository
//   → Returns CompanyDto
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Application.DTOs;
using MediatR;

namespace CompanyService.Application.Commands.CreateCompany;

/// <summary>
/// Command to create a new Company.
/// Sent from the API controller after user submits the Company Setup Wizard.
/// </summary>
public record CreateCompanyCommand(
    string Name,
    string TradeName,
    string Industry,
    string Size,
    CreateCompanyAddressCommand HeadOfficeAddress,
    string ContactEmail,
    string ContactPhone,
    string TaxNumber,
    string? Website
) : IRequest<CompanyDto>;  // Returns CompanyDto when handled

/// <summary>Address nested command</summary>
public record CreateCompanyAddressCommand(
    string Street,
    string? ApartmentSuite,
    string City,
    string State,
    string PostalCode,
    string Country
);