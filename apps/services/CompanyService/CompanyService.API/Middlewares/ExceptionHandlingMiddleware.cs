// ─────────────────────────────────────────────────────────────────────────
// ExceptionHandlingMiddleware.cs (UPDATED)
//
// CHANGE FROM WEEK 1 (Day 4):
//   Added ValidationException handling → HTTP 422 with Errors dictionary.
//   This is what Angular reads to highlight invalid form fields.
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace CompanyService.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        object response;
        HttpStatusCode statusCode;

        switch (exception)
        {
            // ── 422 Unprocessable Entity — validation errors with field details
            case ValidationException validation:
                statusCode = HttpStatusCode.UnprocessableEntity;
                response = new
                {
                    status = (int)statusCode,
                    error = "Validation Failed",
                    message = validation.Message,
                    errors = validation.Errors,   // ← Angular uses this
                    timestamp = DateTime.UtcNow,
                    path = context.Request.Path
                };
                _logger.LogWarning("Validation error: {Errors}", validation.Errors);
                break;

            // ── 404 Not Found
            case NotFoundException notFound:
                statusCode = HttpStatusCode.NotFound;
                response = BuildError(statusCode, notFound.Message, context);
                _logger.LogWarning("Not found: {Message}", notFound.Message);
                break;

            // ── 409 Conflict
            case ConflictException conflict:
                statusCode = HttpStatusCode.Conflict;
                response = BuildError(statusCode, conflict.Message, context);
                _logger.LogWarning("Conflict: {Message}", conflict.Message);
                break;

            // ── 400 Bad Request — general domain rule violation
            case DomainException domain:
                statusCode = HttpStatusCode.BadRequest;
                response = BuildError(statusCode, domain.Message, context);
                _logger.LogWarning("Domain error: {Message}", domain.Message);
                break;

            // ── 500 Internal Server Error — unexpected
            default:
                statusCode = HttpStatusCode.InternalServerError;
                response = BuildError(statusCode, "An unexpected error occurred.", context);
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
    }

    private static object BuildError(HttpStatusCode code, string message, HttpContext context) => new
    {
        status = (int)code,
        error = code.ToString(),
        message,
        timestamp = DateTime.UtcNow,
        path = context.Request.Path
    };
}