// ─────────────────────────────────────────────────────────────────────────
// CompanyDto.cs — Data Transfer Objects (UPDATED for Week 2)
//
// CHANGES FROM WEEK 1 (Day 3):
//   ❌ Removed: bool IsSetupComplete  ← property deleted from Company entity
//   ❌ Removed: bool IsActive         ← property deleted from Company entity
//   ✅ Added:   string Status         ← maps from CompanyStatus enum ("Pending" / "Active" / "Archived")
//   ✅ Added:   bool IsDeleted        ← soft-delete flag added in Week 2
//   ✅ Changed: DateTime UpdatedAt    → DateTime? UpdatedAt (nullable — null until first update)
//
// RULE: DTOs mirror what the API returns. When the entity changes,
//       the DTO and every MapToDto() that builds it must change too.
// ─────────────────────────────────────────────────────────────────────────

namespace CompanyService.Application.DTOs;

/// <summary>
/// Full company details — returned from:
///   GET  /api/v1/companies/{id}
///   POST /api/v1/companies
///   PUT  /api/v1/companies/{id}
/// </summary>
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
    string Status,        // ← "Pending" | "Active" | "Archived"  (was: bool IsSetupComplete + bool IsActive)
    bool IsDeleted,     // ← soft-delete flag (new in Week 2)
    DateTime CreatedAt,
    DateTime? UpdatedAt     // ← nullable — null until company is first updated
);

/// <summary>
/// Compact item for list views.
/// Returned from: GET /api/v1/companies (admin paged list)
/// </summary>
public record CompanyListItemDto(
    Guid Id,
    string Name,
    string TradeName,
    string Industry,
    string Status,          // ← was "bool IsActive" — now shows full status string
    bool IsDeleted
);

/// <summary>
/// Address data nested inside CompanyDto.
/// Matches the Address value object fields exactly.
/// </summary>
public record AddressDto(
    string Street,
    string? ApartmentSuite,
    string City,
    string State,
    string PostalCode,
    string Country
);