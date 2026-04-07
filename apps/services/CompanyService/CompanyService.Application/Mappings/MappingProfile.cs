// ─────────────────────────────────────────────────────────────────────────
// MappingProfile.cs — AutoMapper Configuration
//
// WHY AUTOMAPPER?
//   Every handler (Get, Create, Update) had a private MapToDto() method
//   that manually copied every property one by one. With 10+ properties
//   that's 10+ lines repeated in every handler. If we add a new property
//   (e.g., "LogoUrl") we'd have to update EVERY MapToDto() in every handler.
//
//   AutoMapper does this in one place:
//     CreateMap<Company, CompanyDto>()   ← configured once HERE
//   Every handler just calls: _mapper.Map<CompanyDto>(company)
//   Add a new property → update ONLY this file.
//
// WHAT IT MAPS:
//   Company entity  → CompanyDto  (used by all query/command handlers)
//   Address V.O.    → AddressDto  (nested inside CompanyDto)
//
// CONVENTION:
//   AutoMapper matches properties by NAME automatically.
//   Only properties with DIFFERENT names or types need explicit config.
//   Example: company.Status (enum) → dto.Status (string) — needs MapFrom
// ─────────────────────────────────────────────────────────────────────────

using AutoMapper;
using CompanyService.Application.DTOs;
using CompanyService.Domain.Entities;
using CompanyService.Domain.ValueObjects;

namespace CompanyService.Application.Mappings;

/// <summary>
/// AutoMapper profile — registered once, used by all handlers via IMapper.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ── Company → CompanyDto
        //
        // Properties that match by name map automatically:
        //   Id, Name, TradeName, Industry, TenantId, IsDeleted, CreatedAt, UpdatedAt
        //
        // Properties that need explicit config:
        //   Size   (CompanySize enum) → string ("Small", "Medium" etc.)
        //   Status (CompanyStatus enum) → string ("Pending", "Active", "Archived")
        //
        CreateMap<Company, CompanyDto>()
            .ForMember(
                dest => dest.Size,
                opt => opt.MapFrom(src => src.Size.ToString()))
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));

        // ── Address (Value Object) → AddressDto
        //   All fields match by name — no explicit config needed.
        CreateMap<Address, AddressDto>();
    }
}