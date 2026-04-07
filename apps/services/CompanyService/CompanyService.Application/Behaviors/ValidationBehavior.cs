// ─────────────────────────────────────────────────────────────────────────
// ValidationBehavior.cs — MediatR Pipeline Behavior
//
// WHAT IS A PIPELINE BEHAVIOR?
//   MediatR processes: Request → [Pipeline] → Handler → Response
//   A behavior sits in [Pipeline] and runs BEFORE the handler.
//
//   Think of it like ASP.NET Core middleware, but for MediatR:
//     POST /companies
//       → ExceptionHandlingMiddleware    (ASP.NET Core layer)
//         → ValidationBehavior          (MediatR layer) ← THIS
//           → CreateCompanyCommandHandler               (our handler)
//
// WHAT IT DOES:
//   1. Finds ALL validators for the incoming command (from DI container)
//   2. Runs them all
//   3. If ANY rule fails → throws ValidationException with all errors
//   4. If ALL pass → calls the handler normally
//
// RESULT:
//   You never have to call validator.Validate(command) inside handlers.
//   Validation is automatic for every command that has a validator.
// ─────────────────────────────────────────────────────────────────────────

using FluentValidation;
using MediatR;
using CompanyService.Domain.Exceptions;
using ValidationException = CompanyService.Domain.Exceptions.ValidationException;

namespace CompanyService.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that validates every command before its handler runs.
/// Registered in DI as a generic open type — applies to ALL IRequest<T>.
/// </summary>
public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    // All validators registered for TRequest (could be 0 or more)
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // If no validators are registered for this command, skip straight to handler
        if (!_validators.Any())
            return await next();

        // Run all validators
        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f is not null)
            .ToList();

        // If no failures, proceed to handler
        if (failures.Count == 0)
            return await next();

        // Build errors dictionary: { "name": ["Name is required.", "Max 200 chars."] }
        // Key = property name in camelCase (matches Angular form control names)
        var errors = failures
            .GroupBy(f => ToCamelCase(f.PropertyName))
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray());

        // Throw our domain ValidationException — ExceptionHandlingMiddleware
        // catches this and returns HTTP 422 with the errors dictionary
        throw new ValidationException(errors);
    }

    // Convert "HeadOfficeAddress.Street" → "headOfficeAddress.street"
    private static string ToCamelCase(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName)) return propertyName;
        var parts = propertyName.Split('.');
        return string.Join(".", parts.Select(p =>
            char.ToLowerInvariant(p[0]) + p[1..]));
    }
}