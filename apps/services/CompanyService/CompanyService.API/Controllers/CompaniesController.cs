// ─────────────────────────────────────────────────────────────────────────
// CompaniesController.cs — REST API Endpoints
//
// The API layer is thin — it just:
//   1. Receives HTTP requests
//   2. Sends them as MediatR commands/queries
//   3. Returns HTTP responses
// ALL business logic is in Application/Domain layers.
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Application.Commands.CreateCompany;
using CompanyService.Application.DTOs;
using CompanyService.Application.Queries.GetCompany;
using CompanyService.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CompanyService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CompaniesController> _logger;

    public CompaniesController(IMediator mediator, ILogger<CompaniesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Get a specific company by ID</summary>
    /// <param name="id">Company GUID</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching company {CompanyId}", id);

        var query = new GetCompanyQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>Create a new company</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request,CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating company: {CompanyName}", request.Name);

        var command = new CreateCompanyCommand(
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
}

// ── Request DTOs (what comes IN from the HTTP request body)
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

public record CreateAddressRequest(
    string Street,
    string? ApartmentSuite,
    string City,
    string State,
    string PostalCode,
    string Country
);