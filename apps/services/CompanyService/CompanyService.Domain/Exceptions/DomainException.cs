namespace CompanyService.Domain.Exceptions;

/// <summary>
/// Thrown when a business rule is violated.
/// Example: "Company name cannot be empty"
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown when an entity cannot be found in the repository.
/// Results in a 404 Not Found HTTP response.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.") { }
}

/// <summary>
/// Thrown when an operation conflicts with existing data.
/// Example: "A company with this tax number already exists."
/// Results in a 409 Conflict HTTP response.
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message) { }
}