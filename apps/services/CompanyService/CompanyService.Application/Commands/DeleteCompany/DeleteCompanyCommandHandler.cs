using CompanyService.Domain.Entities;
using CompanyService.Domain.Exceptions;
using CompanyService.Domain.Interfaces;
using MediatR;

namespace CompanyService.Application.Commands.DeleteCompany;

public sealed class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand>
{
    private readonly ICompanyRepository _repository;

    public DeleteCompanyCommandHandler(ICompanyRepository repository)
        => _repository = repository;

    public async Task Handle(DeleteCompanyCommand command, CancellationToken cancellationToken)
    {
        var exists = await _repository.ExistsByIdAsync(command.CompanyId, cancellationToken);
        if (!exists)
            throw new NotFoundException(nameof(Company), command.CompanyId);

        await _repository.DeleteAsync(command.CompanyId, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}