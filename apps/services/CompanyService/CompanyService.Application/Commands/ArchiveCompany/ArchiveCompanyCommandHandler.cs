using CompanyService.Domain.Entities;
using CompanyService.Domain.Exceptions;
using CompanyService.Domain.Interfaces;
using MediatR;

namespace CompanyService.Application.Commands.ArchiveCompany;

public sealed class ArchiveCompanyCommandHandler : IRequestHandler<ArchiveCompanyCommand>
{
    private readonly ICompanyRepository _repository;

    public ArchiveCompanyCommandHandler(ICompanyRepository repository)
        => _repository = repository;

    public async Task Handle(ArchiveCompanyCommand command, CancellationToken cancellationToken)
    {
        var company = await _repository.GetByIdAsync(command.CompanyId, cancellationToken);
        if (company is null)
            throw new NotFoundException(nameof(Company), command.CompanyId);

        company.Archive();   // Enforces: must be Active to archive
        await _repository.UpdateAsync(company, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}