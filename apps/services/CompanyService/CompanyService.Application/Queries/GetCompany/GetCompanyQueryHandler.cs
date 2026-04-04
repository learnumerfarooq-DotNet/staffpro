using CompanyService.Application.DTOs;
using CompanyService.Domain.Exceptions;
using CompanyService.Domain.Interfaces;
using MediatR;

namespace CompanyService.Application.Queries.GetCompany;

// ── Query definition
public record GetCompanyQuery(Guid CompanyId) : IRequest<CompanyDto>;

/// <summary>
/// Handles retrieving a single company by ID.
/// </summary>
public sealed class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, CompanyDto>
{
    private readonly ICompanyRepository _repository;

    public GetCompanyQueryHandler(ICompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<CompanyDto> Handle(GetCompanyQuery query, CancellationToken cancellationToken)
    {
        var company = await _repository.GetByIdAsync(query.CompanyId, cancellationToken);

        if (company is null)
            throw new NotFoundException(nameof(company), query.CompanyId);

        return new CompanyDto(
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
            company.UpdatedAt);
    }
}