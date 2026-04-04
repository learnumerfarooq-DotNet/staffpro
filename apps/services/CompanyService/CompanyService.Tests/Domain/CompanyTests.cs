// ─────────────────────────────────────────────────────────────────────────
// CompanyTests.cs — Unit Tests for Company Domain Entity
//
// Tests verify that ALL business rules in the Company entity work correctly.
// Rule: 1 test = 1 business rule.
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Entities;
using CompanyService.Domain.Exceptions;
using CompanyService.Domain.ValueObjects;

namespace CompanyService.Tests.Domain;

public class CompanyTests
{
    // ── Helper to create a valid address for tests
    private static Address ValidAddress() => Address.Create(
        street: "123 Main St",
        city: "New York",
        state: "NY",
        postalCode: "10001",
        country: "USA");

    // ────────────────────────────────────────────
    // CREATION TESTS
    // ────────────────────────────────────────────

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var company = Company.Create(
            name: "TechCorp Inc.",
            tradeName: "TechCorp",
            industry: "Technology",
            size: CompanySize.Medium,
            headOfficeAddress: ValidAddress(),
            contactEmail: "info@techcorp.com",
            contactPhone: "+1-555-123-4567",
            taxNumber: "TC-123456");

        // Assert
        Assert.NotEqual(Guid.Empty, company.Id);
        Assert.Equal("TechCorp Inc.", company.Name);
        Assert.Equal("info@techcorp.com", company.ContactEmail);
        Assert.True(company.IsActive);
        Assert.False(company.IsSetupComplete);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            Company.Create(
                name: "",        // ← Empty name — should fail
                tradeName: "T",
                industry: "Tech",
                size: CompanySize.Small,
                headOfficeAddress: ValidAddress(),
                contactEmail: "test@test.com",
                contactPhone: "+1234567890",
                taxNumber: "TX-001"));

        Assert.Contains("Company name is required", exception.Message);
    }

    [Fact]
    public void Create_WithInvalidEmail_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            Company.Create(
                name: "Test Co",
                tradeName: "Test",
                industry: "Tech",
                size: CompanySize.Small,
                headOfficeAddress: ValidAddress(),
                contactEmail: "not-a-valid-email",  // ← Invalid email
                contactPhone: "+1234567890",
                taxNumber: "TX-001"));

        Assert.Contains("valid email", exception.Message);
    }

    [Fact]
    public void Create_ShouldRaiseDomainEvent()
    {
        // Act
        var company = Company.Create(
            name: "EventTest Co",
            tradeName: "EventTest",
            industry: "Tech",
            size: CompanySize.Small,
            headOfficeAddress: ValidAddress(),
            contactEmail: "event@test.com",
            contactPhone: "+1234567890",
            taxNumber: "TX-EVT-001");

        // Assert
        Assert.Single(company.DomainEvents);
        Assert.IsType<CompanyService.Domain.Events.CompanyCreatedEvent>(company.DomainEvents[0]);
    }

    // ────────────────────────────────────────────
    // BUSINESS OPERATION TESTS
    // ────────────────────────────────────────────

    [Fact]
    public void CompleteSetup_WhenNotComplete_ShouldSucceed()
    {
        var company = Company.Create("Co", "Co", "Tech", CompanySize.Small,
            ValidAddress(), "a@b.com", "+123", "TX-001");

        company.CompleteSetup();

        Assert.True(company.IsSetupComplete);
    }

    [Fact]
    public void CompleteSetup_WhenAlreadyComplete_ShouldThrow()
    {
        var company = Company.Create("Co", "Co", "Tech", CompanySize.Small,
            ValidAddress(), "a@b.com", "+123", "TX-001");

        company.CompleteSetup();  // First call — OK

        Assert.Throws<DomainException>(() => company.CompleteSetup()); // Second — should throw
    }

    [Fact]
    public void Deactivate_WhenActive_ShouldDeactivate()
    {
        var company = Company.Create("Co", "Co", "Tech", CompanySize.Small,
            ValidAddress(), "a@b.com", "+123", "TX-001");

        company.Deactivate();

        Assert.False(company.IsActive);
    }
}