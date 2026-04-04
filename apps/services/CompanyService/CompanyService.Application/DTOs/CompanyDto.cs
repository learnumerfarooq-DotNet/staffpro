// ─────────────────────────────────────────────────────────────────────────
// CompanyDto.cs — Data Transfer Objects
//
// DTOs are simple data containers with no business logic.
// They are used to transfer data between the API layer and clients.
// They hide internal entity structure from the outside world.
// ─────────────────────────────────────────────────────────────────────────

namespace CompanyService.Application.DTOs;

/// <summary>Full company details (returned from GET /companies/{id})</summary>
public record CompanyDto(
    Guid Id,
    string Name,
    string TradeName,
    string Industry,
    string Size,
    AddressDto HeadOfficeAddress,
    string ContactEmail,
    string ContactPhone,
    string? Website,
    string TaxNumber,
    bool IsSetupComplete,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>Compact company list item (returned from GET /companies)</summary>
public record CompanyListItemDto(
    Guid Id,
    string Name,
    string TradeName,
    string Industry,
    bool IsActive
);

/// <summary>Address DTO — used inside CompanyDto</summary>
public record AddressDto(
    string Street,
    string? ApartmentSuite,
    string City,
    string State,
    string PostalCode,
    string Country
);