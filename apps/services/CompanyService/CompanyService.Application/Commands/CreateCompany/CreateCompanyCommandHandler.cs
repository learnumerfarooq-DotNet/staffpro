using AutoMapper;
using CompanyService.Application.DTOs;
using CompanyService.Domain.Entities;
using CompanyService.Domain.Exceptions;
using CompanyService.Domain.Interfaces;
using CompanyService.Domain.ValueObjects;
using MediatR;

namespace CompanyService.Application.Commands.CreateCompany;

public sealed class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, CompanyDto>
{
    private readonly ICompanyRepository _repository;
    private readonly IMapper _mapper;              // ← NEW: injected by DI

    public CreateCompanyCommandHandler(ICompanyRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CompanyDto> Handle(CreateCompanyCommand command, CancellationToken cancellationToken)
    {
        var exists = await _repository.ExistsByTaxNumberAsync(command.TaxNumber, cancellationToken);
        if (exists)
            throw new ConflictException($"A company with tax number '{command.TaxNumber}' already exists.");

        if (!Enum.TryParse<CompanySize>(command.Size, ignoreCase: true, out var companySize))
            throw new DomainException($"Invalid company size: '{command.Size}'.");

        var address = Address.Create(
            command.HeadOfficeAddress.Street,
            command.HeadOfficeAddress.City,
            command.HeadOfficeAddress.State,
            command.HeadOfficeAddress.PostalCode,
            command.HeadOfficeAddress.Country,
            command.HeadOfficeAddress.ApartmentSuite);

        var company = Company.Create(
            command.TenantId,
            command.Name,
            command.TradeName,
            command.Industry,
            companySize,
            address,
            command.ContactEmail,
            command.ContactPhone,
            command.TaxNumber,
            command.Website);

        await _repository.AddAsync(company, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // ← REPLACED: was private MapToDto() method
        return _mapper.Map<CompanyDto>(company);
    }
}