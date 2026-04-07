using AutoMapper;
using CompanyService.Application.DTOs;
using CompanyService.Domain.Exceptions;
using CompanyService.Domain.Interfaces;
using MediatR;

namespace CompanyService.Application.Queries.GetCompany;

public record GetCompanyQuery(Guid CompanyId) : IRequest<CompanyDto>;

public sealed class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, CompanyDto>
{
    private readonly ICompanyRepository _repository;
    private readonly IMapper _mapper;

    public GetCompanyQueryHandler(ICompanyRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CompanyDto> Handle(GetCompanyQuery query, CancellationToken cancellationToken)
    {
        var company = await _repository.GetByIdAsync(query.CompanyId, cancellationToken);

        if (company is null)
            throw new NotFoundException(nameof(company), query.CompanyId);

        // ← REPLACED: was manual new CompanyDto(...) with all fields
        return _mapper.Map<CompanyDto>(company);
    }
}