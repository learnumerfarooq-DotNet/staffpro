// ─────────────────────────────────────────────────────────────────────────
// DomainException.cs — Exception Hierarchy (UPGRADED)
//
// EXISTING EXCEPTIONS (from Week 1, Day 3):
//   DomainException   → base class for all domain errors
//   NotFoundException → entity not found → HTTP 404
//   ConflictException → duplicate data   → HTTP 409
//
// NEW TODAY:
//   ValidationException → input validation errors → HTTP 422
//
// WHY ValidationException?
//   FluentValidation (added in Week 3) runs before command handlers.
//   When validation fails, it throws a ValidationException with a list
//   of errors like: ["Name is required", "Email is not valid"].
//   The ExceptionHandlingMiddleware (from Day 4) catches this and returns
//   HTTP 422 with all field errors — exactly what Angular needs to
//   highlight the invalid form fields.
//
// EXCEPTION → HTTP STATUS MAPPING (ExceptionHandlingMiddleware):
//   DomainException      → 400 Bad Request
//   NotFoundException    → 404 Not Found
//   ConflictException    → 409 Conflict
//   ValidationException  → 422 Unprocessable Entity  ← NEW TODAY
// ─────────────────────────────────────────────────────────────────────────

namespace CompanyService.Domain.Exceptions;

/// <summary>
/// Base exception for all business rule violations in StaffPro.
/// Thrown when an entity's business rules are violated.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown when a requested entity does not exist.
/// The ExceptionHandlingMiddleware maps this to HTTP 404 Not Found.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.") { }
}

/// <summary>
/// Thrown when an operation would create a duplicate (e.g. duplicate tax number).
/// The ExceptionHandlingMiddleware maps this to HTTP 409 Conflict.
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message) { }
}

/// <summary>
/// Thrown when FluentValidation detects malformed input.
/// Contains a dictionary of field → error messages for the Angular form.
/// The ExceptionHandlingMiddleware maps this to HTTP 422 Unprocessable Entity.
///
/// Example errors dictionary:
/// {
///   "name"          : ["Name is required.", "Name cannot exceed 200 characters."],
///   "contactEmail"  : ["Contact email is not a valid email address."],
///   "taxNumber"     : ["Tax number is required."]
/// }
/// </summary>
public class ValidationException : DomainException
{
    /// <summary>
    /// Field-level validation errors.
    /// Key = property name (camelCase to match Angular form controls).
    /// Value = list of error messages for that field.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
        => Errors = errors;
}