using MediatR;

namespace CompanyService.Application.Commands.ArchiveCompany;

/// <summary>Command to archive (suspend) an active company.</summary>
public record ArchiveCompanyCommand(Guid CompanyId) : IRequest;