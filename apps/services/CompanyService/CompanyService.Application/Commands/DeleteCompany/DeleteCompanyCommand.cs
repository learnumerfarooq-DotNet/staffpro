using MediatR;

namespace CompanyService.Application.Commands.DeleteCompany;

/// <summary>Command to soft-delete a company (sets IsDeleted = true).</summary>
public record DeleteCompanyCommand(Guid CompanyId) : IRequest;