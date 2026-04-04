using CompanyService.Application.DTOs;
using CompanyService.Domain.Entities;
using CompanyService.Domain.Exceptions;
using CompanyService.Domain.Interfaces;
using CompanyService.Domain.ValueObjects;
using MediatR;

namespace CompanyService.Application.Commands.CreateCompany;

/// <summary>
/// Handles the CreateCompanyCommand.
/// This is the "use case" — all business orchestration happens here.
/// </summary>
public sealed class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, CompanyDto>
{
    private readonly ICompanyRepository _repository;

    public CreateCompanyCommandHandler(ICompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<CompanyDto> Handle(
        CreateCompanyCommand command,
        CancellationToken cancellationToken)
    {
        // ── 1. Check for duplicate tax number
        var exists = await _repository.ExistsByTaxNumberAsync(command.TaxNumber, cancellationToken);
        if (exists)
            throw new ConflictException($"A company with tax number '{command.TaxNumber}' already exists.");

        // ── 2. Parse the company size enum
        if (!Enum.TryParse<CompanySize>(command.Size, ignoreCase: true, out var companySize))
            throw new DomainException($"Invalid company size: '{command.Size}'.");

        // ── 3. Create the Address value object (validates internally)
        var address = Address.Create(
            command.HeadOfficeAddress.Street,
            command.HeadOfficeAddress.City,
            command.HeadOfficeAddress.State,
            command.HeadOfficeAddress.PostalCode,
            command.HeadOfficeAddress.Country,
            command.HeadOfficeAddress.ApartmentSuite);

        // ── 4. Create the Company entity (validates business rules internally)
        var company = Company.Create(
            command.Name,
            command.TradeName,
            command.Industry,
            companySize,
            address,
            command.ContactEmail,
            command.ContactPhone,
            command.TaxNumber,
            command.Website);

        // ── 5. Persist to the database
        await _repository.AddAsync(company, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // ── 6. Return the DTO (never return the entity directly to API layer)
        return MapToDto(company);
    }

    private static CompanyDto MapToDto(Company company) => new(
        company.Id,
        company.Name,
        company.TradeName,
        company.Industry,
        company.Size.ToString(),
        new AddressDto(
            company.HeadOfficeAddress.Street,
            company.HeadOfficeAddress.ApartmentSuite,
            company.HeadOfficeAddress.City,
            company.HeadOfficeAddress.State,
            company.HeadOfficeAddress.PostalCode,
            company.HeadOfficeAddress.Country),
        company.ContactEmail,
        company.ContactPhone,
        company.Website,
        company.TaxNumber,
        company.IsSetupComplete,
        company.IsActive,
        company.CreatedAt,
        company.UpdatedAt
    );
}