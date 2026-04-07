// ─────────────────────────────────────────────────────────────────────────
// CreateCompanyCommandValidator.cs — FluentValidation Rules
//
// WHERE DOES THIS RUN?
//   BEFORE the handler. The ValidationBehavior (below) intercepts every
//   command, finds its validator, runs it, and throws ValidationException
//   if any rule fails. The handler never even executes for invalid input.
//
// TWO LAYERS OF VALIDATION:
//   1. FluentValidation (here)  — "Is the input well-formed?"
//        Name not empty, Email format correct, TaxNumber present
//        → Fails fast, no DB call
//   2. Domain rules (Company.Create) — "Does it break a business rule?"
//        TaxNumber unique (needs DB check), name <= 200 chars
//        → Only runs if FluentValidation passes
// ─────────────────────────────────────────────────────────────────────────

using FluentValidation;

namespace CompanyService.Application.Commands.CreateCompany;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        // ── TenantId must be a real Guid
        RuleFor(c => c.TenantId)
            .NotEqual(Guid.Empty)
            .WithMessage("TenantId cannot be empty.");

        // ── Company Name
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(200).WithMessage("Company name cannot exceed 200 characters.");

        // ── Trade Name
        RuleFor(c => c.TradeName)
            .NotEmpty().WithMessage("Trade name is required.")
            .MaximumLength(200).WithMessage("Trade name cannot exceed 200 characters.");

        // ── Industry
        RuleFor(c => c.Industry)
            .NotEmpty().WithMessage("Industry is required.")
            .MaximumLength(100).WithMessage("Industry cannot exceed 100 characters.");

        // ── Size — must be one of the valid CompanySize enum names
        RuleFor(c => c.Size)
            .NotEmpty().WithMessage("Company size is required.")
            .Must(s => Enum.TryParse<Domain.Entities.CompanySize>(s, ignoreCase: true, out _))
            .WithMessage("Size must be one of: Startup, Small, Medium, Large, Enterprise.");

        // ── Contact Email — format check (domain does exact validation)
        RuleFor(c => c.ContactEmail)
            .NotEmpty().WithMessage("Contact email is required.")
            .EmailAddress().WithMessage("Contact email is not a valid email address.")
            .MaximumLength(254).WithMessage("Contact email cannot exceed 254 characters.");

        // ── Contact Phone
        RuleFor(c => c.ContactPhone)
            .NotEmpty().WithMessage("Contact phone is required.")
            .MaximumLength(30).WithMessage("Contact phone cannot exceed 30 characters.");

        // ── Tax Number
        RuleFor(c => c.TaxNumber)
            .NotEmpty().WithMessage("Tax number is required.")
            .MaximumLength(50).WithMessage("Tax number cannot exceed 50 characters.");

        // ── Address: Street
        RuleFor(c => c.HeadOfficeAddress.Street)
            .NotEmpty().WithMessage("Street address is required.")
            .MaximumLength(300).WithMessage("Street cannot exceed 300 characters.");

        // ── Address: City
        RuleFor(c => c.HeadOfficeAddress.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

        // ── Address: Country
        RuleFor(c => c.HeadOfficeAddress.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters.");

        // ── Address: Postal Code
        RuleFor(c => c.HeadOfficeAddress.PostalCode)
            .NotEmpty().WithMessage("Postal code is required.")
            .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters.");
    }
}