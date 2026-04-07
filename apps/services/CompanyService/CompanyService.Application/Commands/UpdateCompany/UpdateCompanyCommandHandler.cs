using AutoMapper;
using CompanyService.Application.DTOs;
using CompanyService.Domain.Entities;
using CompanyService.Domain.Exceptions;
using CompanyService.Domain.Interfaces;
using CompanyService.Domain.ValueObjects;
using MediatR;

namespace CompanyService.Application.Commands.UpdateCompany;

public sealed class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, CompanyDto>
{
    private readonly ICompanyRepository _repository;
    private readonly IMapper _mapper;

    public UpdateCompanyCommandHandler(ICompanyRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CompanyDto> Handle(UpdateCompanyCommand command, CancellationToken cancellationToken)
    {
        var company = await _repository.GetByIdAsync(command.CompanyId, cancellationToken);
        if (company is null)
            throw new NotFoundException(nameof(Company), command.CompanyId);

        if (!Enum.TryParse<CompanySize>(command.Size, ignoreCase: true, out var size))
            throw new DomainException($"Invalid company size: '{command.Size}'.");

        var address = Address.Create(
            command.HeadOfficeAddress.Street,
            command.HeadOfficeAddress.City,
            command.HeadOfficeAddress.State,
            command.HeadOfficeAddress.PostalCode,
            command.HeadOfficeAddress.Country,
            command.HeadOfficeAddress.ApartmentSuite);

        company.UpdateDetails(command.Name, command.TradeName, command.Industry, size);
        company.UpdateHeadOffice(address);
        company.UpdateContactInfo(command.ContactEmail, command.ContactPhone, command.Website);

        await _repository.UpdateAsync(company, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // ← REPLACED: was manual MapToDto() with Status booleans error
        return _mapper.Map<CompanyDto>(company);
    }
}