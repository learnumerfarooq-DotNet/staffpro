using CompanyService.Application.DTOs;
using MediatR;

namespace CompanyService.Application.Commands.CreateCompany;

/// <summary>
/// Command to create a new Company.
/// TenantId added — Company.Create() requires it (added in Week 2 Day 8).
/// Week 4 will supply the real TenantId from the JWT token via the controller.
/// </summary>
public record CreateCompanyCommand(
    Guid TenantId,          // ← identifies which tenant is registering
    string Name,
    string TradeName,
    string Industry,
    string Size,
    CreateCompanyAddressCommand HeadOfficeAddress,
    string ContactEmail,
    string ContactPhone,
    string TaxNumber,
    string? Website
) : IRequest<CompanyDto>;

/// <summary>Address nested inside the create command.</summary>
public record CreateCompanyAddressCommand(
    string Street,
    string? ApartmentSuite,
    string City,
    string State,
    string PostalCode,
    string Country
);