using CompanyService.Application.Commands.ArchiveCompany;
using CompanyService.Application.Commands.CreateCompany;
using CompanyService.Application.Commands.DeleteCompany;
using CompanyService.Application.Commands.UpdateCompany;
using CompanyService.Application.DTOs;
using CompanyService.Application.Queries.GetCompany;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CompanyService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CompaniesController> _logger;
    private readonly IConfiguration _configuration;   // ← needed to read placeholder TenantId

    public CompaniesController(
        IMediator mediator,
        ILogger<CompaniesController> logger,
        IConfiguration configuration)
    {
        _mediator = mediator;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>Get a company by ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching company {CompanyId}", id);
        var result = await _mediator.Send(new GetCompanyQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Create a new company</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCompanyRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating company: {CompanyName}", request.Name);

        // TODO Week 4: replace with real TenantId from JWT token:
        //   var tenantId = Guid.Parse(User.FindFirst("tenantId")!.Value);
        var tenantId = Guid.Parse(
            _configuration["MultiTenancy:DefaultTenantId"]
            ?? "00000000-0000-0000-0000-000000000001");  // ← placeholder, never Guid.Empty

        var command = new CreateCompanyCommand(
            tenantId,               // ← FIX: was Guid.Empty which throws DomainException
            request.Name,
            request.TradeName,
            request.Industry,
            request.Size,
            new CreateCompanyAddressCommand(
                request.HeadOfficeAddress.Street,
                request.HeadOfficeAddress.ApartmentSuite,
                request.HeadOfficeAddress.City,
                request.HeadOfficeAddress.State,
                request.HeadOfficeAddress.PostalCode,
                request.HeadOfficeAddress.Country),
            request.ContactEmail,
            request.ContactPhone,
            request.TaxNumber,
            request.Website);

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Update a company's details</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCompanyRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCompanyCommand(
            id,
            request.Name,
            request.TradeName,
            request.Industry,
            request.Size,
            new UpdateCompanyAddressCommand(
                request.HeadOfficeAddress.Street,
                request.HeadOfficeAddress.ApartmentSuite,
                request.HeadOfficeAddress.City,
                request.HeadOfficeAddress.State,
                request.HeadOfficeAddress.PostalCode,
                request.HeadOfficeAddress.Country),
            request.ContactEmail,
            request.ContactPhone,
            request.Website);

        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>Archive (suspend) a company</summary>
    [HttpPost("{id:guid}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Archive(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ArchiveCompanyCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Soft-delete a company</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCompanyCommand(id), cancellationToken);
        return NoContent();
    }
}

// ── Incoming HTTP request body shapes

public record CreateCompanyRequest(
    string Name,
    string TradeName,
    string Industry,
    string Size,
    CreateAddressRequest HeadOfficeAddress,
    string ContactEmail,
    string ContactPhone,
    string TaxNumber,
    string? Website
);

public record UpdateCompanyRequest(
    string Name,
    string TradeName,
    string Industry,
    string Size,
    CreateAddressRequest HeadOfficeAddress,
    string ContactEmail,
    string ContactPhone,
    string? Website
);

public record CreateAddressRequest(
    string Street,
    string? ApartmentSuite,
    string City,
    string State,
    string PostalCode,
    string Country
);