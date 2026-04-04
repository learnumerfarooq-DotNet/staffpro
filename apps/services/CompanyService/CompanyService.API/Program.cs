// ─────────────────────────────────────────────────────────────────────────
// Program.cs — Application Entry Point
// In .NET 10, this is the top-level file that bootstraps the app.
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.API.Middlewares;
using CompanyService.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// ────────────────────────────────────────────
// REGISTER SERVICES (Dependency Injection)
// ────────────────────────────────────────────

// ASP.NET Core controllers
builder.Services.AddControllers();

// Swagger / OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "CompanyService API",
        Version = "v1",
        Description = "Manages company registration and profile for StaffPro."
    });
});

// CORS — Allow Angular frontend to call this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// MediatR — CQRS dispatcher (auto-discovers handlers in Application assembly)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(CompanyService.Application.Commands.CreateCompany.CreateCompanyCommand).Assembly));

// Infrastructure layer (EF Core + repositories)
builder.Services.AddInfrastructure(builder.Configuration);

// Health checks
builder.Services.AddHealthChecks().AddDbContextCheck<CompanyService.Infrastructure.Data.CompanyDbContext>();

// ────────────────────────────────────────────
// BUILD & CONFIGURE PIPELINE
// ────────────────────────────────────────────

var app = builder.Build();

// Development-only features
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CompanyService v1");
        options.RoutePrefix = string.Empty; // Swagger at http://localhost:5001/
    });
}

// Global exception handler
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowAngularDev");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();