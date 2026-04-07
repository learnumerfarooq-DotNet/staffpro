// ─────────────────────────────────────────────────────────────────────────
// CompanyTests.cs — Unit Tests for the Company Aggregate Root
//
// NAMING CONVENTION:
//   MethodName_Scenario_ExpectedResult
//   Examples:
//     Create_WithValidData_ReturnsCompanyWithPendingStatus
//     CompleteSetup_WhenAlreadyActive_ThrowsDomainException
//
// TEST PATTERN — AAA (Arrange, Act, Assert):
//   Arrange = set up the preconditions
//   Act     = call the method under test
//   Assert  = check the result with FluentAssertions
//
// KEY RULE: Each test has ONE reason to fail. Test one thing per test.
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Common;
using CompanyService.Domain.Entities;
using CompanyService.Domain.Events;
using CompanyService.Domain.Exceptions;
using CompanyService.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CompanyService.Tests.Domain;

public class CompanyTests
{
    // ─────────────────────────────────────────────────
    // Shared test helpers — avoid repeating Arrange code
    // ─────────────────────────────────────────────────

    private static readonly Guid ValidTenantId = Guid.NewGuid();

    private static Address ValidAddress() =>
        Address.Create("24 Baker Street", "London", "England", "W1U 3BW", "United Kingdom");

    private static Company CreateValidCompany() =>
        Company.Create(
            ValidTenantId,
            name: "TechCorp Ltd",
            tradeName: "TechCorp",
            industry: "Technology",
            size: CompanySize.Small,
            headOfficeAddress: ValidAddress(),
            contactEmail: "admin@techcorp.com",
            contactPhone: "+44-7000-123456",
            taxNumber: "TAX-UK-001");

    // ═══════════════════════════════════════════════
    // CREATE TESTS
    // ═══════════════════════════════════════════════

    [Fact]
    public void Create_WithValidData_ReturnsCompanyWithPendingStatus()
    {
        // Act
        var company = CreateValidCompany();

        // Assert
        company.Should().NotBeNull();
        company.Name.Should().Be("TechCorp Ltd");
        company.TradeName.Should().Be("TechCorp");
        company.Industry.Should().Be("Technology");
        company.TenantId.Should().Be(ValidTenantId);
        company.Status.Should().Be(CompanyStatus.Pending);   // ← always starts Pending
        company.IsDeleted.Should().BeFalse();
        company.Id.Should().NotBeEmpty();
        company.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        company.UpdatedAt.Should().BeNull();  // ← null until first update
    }

    [Fact]
    public void Create_WithValidData_RaisesCompanyCreatedEvent()
    {
        // Act
        var company = CreateValidCompany();

        // Assert — domain event was raised
        company.DomainEvents.Should().ContainSingle();
        company.DomainEvents.First().Should().BeOfType<CompanyCreatedEvent>();

        var evt = (CompanyCreatedEvent)company.DomainEvents.First();
        evt.CompanyId.Should().Be(company.Id);
        evt.CompanyName.Should().Be("TechCorp Ltd");
        evt.ContactEmail.Should().Be("admin@techcorp.com");
    }

    [Fact]
    public void Create_WithEmptyTenantId_ThrowsDomainException()
    {
        // Arrange
        var emptyTenantId = Guid.Empty;

        // Act
        var act = () => Company.Create(emptyTenantId, "TechCorp", "TechCo", "IT",
            CompanySize.Small, ValidAddress(), "admin@tc.com", "+1-555-0000", "TAX-001");

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("*TenantId*");
    }

    [Theory]
    [InlineData("")]          // empty string
    [InlineData("   ")]       // whitespace only
    [InlineData(null)]        // null
    public void Create_WithInvalidName_ThrowsDomainException(string? invalidName)
    {
        var act = () => Company.Create(ValidTenantId, invalidName!, "TechCo", "IT",
            CompanySize.Small, ValidAddress(), "admin@tc.com", "+1-555-0000", "TAX-001");

        act.Should().Throw<DomainException>()
           .WithMessage("*name*");
    }

    [Fact]
    public void Create_WithNameExceeding200Chars_ThrowsDomainException()
    {
        // Arrange — 201 character name
        var longName = new string('A', 201);

        var act = () => Company.Create(ValidTenantId, longName, "TechCo", "IT",
            CompanySize.Small, ValidAddress(), "admin@tc.com", "+1-555-0000", "TAX-001");

        act.Should().Throw<DomainException>()
           .WithMessage("*200*");
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    [InlineData("")]
    public void Create_WithInvalidEmail_ThrowsDomainException(string invalidEmail)
    {
        var act = () => Company.Create(ValidTenantId, "TechCorp", "TechCo", "IT",
            CompanySize.Small, ValidAddress(), invalidEmail, "+1-555-0000", "TAX-001");

        act.Should().Throw<DomainException>()
           .WithMessage("*email*");
    }

    [Fact]
    public void Create_TrimsWhitespaceFromInputs()
    {
        var company = Company.Create(ValidTenantId,
            "  TechCorp Ltd  ", "  TechCorp  ", "  Technology  ",
            CompanySize.Small, ValidAddress(),
            "admin@techcorp.com", "+44-7000-123456", "  TAX-UK-001  ");

        company.Name.Should().Be("TechCorp Ltd");        // trimmed
        company.TradeName.Should().Be("TechCorp");       // trimmed
        company.Industry.Should().Be("Technology");      // trimmed
        company.TaxNumber.Should().Be("TAX-UK-001");     // trimmed
    }

    [Fact]
    public void Create_NormalisesEmailToLowerCase()
    {
        var company = Company.Create(ValidTenantId,
            "TechCorp", "TechCo", "IT", CompanySize.Small,
            ValidAddress(), "ADMIN@TechCorp.COM", "+44-7000-0000", "TAX-001");

        company.ContactEmail.Should().Be("admin@techcorp.com");  // lowercased
    }

    // ═══════════════════════════════════════════════
    // LIFECYCLE TESTS
    // ═══════════════════════════════════════════════

    [Fact]
    public void CompleteSetup_WhenPending_SetsStatusToActive()
    {
        // Arrange
        var company = CreateValidCompany();
        company.ClearDomainEvents();

        // Act
        company.CompleteSetup();

        // Assert
        company.Status.Should().Be(CompanyStatus.Active);
        company.UpdatedAt.Should().NotBeNull();
        company.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<CompanySetupCompletedEvent>();
    }

    [Fact]
    public void CompleteSetup_WhenAlreadyActive_ThrowsDomainException()
    {
        var company = CreateValidCompany();
        company.CompleteSetup();   // first time → OK

        // Act — second time → should throw
        var act = () => company.CompleteSetup();

        act.Should().Throw<DomainException>()
           .WithMessage("*Pending*");
    }

    [Fact]
    public void Archive_WhenActive_SetsStatusToArchived()
    {
        var company = CreateValidCompany();
        company.CompleteSetup();     // Pending → Active
        company.ClearDomainEvents();

        company.Archive();

        company.Status.Should().Be(CompanyStatus.Archived);
        company.UpdatedAt.Should().NotBeNull();
        company.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<CompanyArchivedEvent>();
    }

    [Fact]
    public void Archive_WhenPending_ThrowsDomainException()
    {
        var company = CreateValidCompany();  // Status = Pending

        var act = () => company.Archive();

        act.Should().Throw<DomainException>()
           .WithMessage("*not completed setup*");
    }

    [Fact]
    public void Archive_WhenAlreadyArchived_ThrowsDomainException()
    {
        var company = CreateValidCompany();
        company.CompleteSetup();
        company.Archive();           // first archive → OK

        var act = () => company.Archive();  // second → should throw

        act.Should().Throw<DomainException>()
           .WithMessage("*already archived*");
    }

    [Fact]
    public void Reactivate_WhenArchived_SetsStatusToActive()
    {
        var company = CreateValidCompany();
        company.CompleteSetup();
        company.Archive();

        company.Reactivate();

        company.Status.Should().Be(CompanyStatus.Active);
    }

    [Fact]
    public void Reactivate_WhenNotArchived_ThrowsDomainException()
    {
        var company = CreateValidCompany();  // Status = Pending

        var act = () => company.Reactivate();

        act.Should().Throw<DomainException>()
           .WithMessage("*Archived*");
    }

    [Fact]
    public void Delete_SetsIsDeletedTrue_AndRaisesEvent()
    {
        var company = CreateValidCompany();
        company.ClearDomainEvents();

        company.Delete();

        company.IsDeleted.Should().BeTrue();
        company.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<CompanyDeletedEvent>();
    }

    [Fact]
    public void Delete_WhenAlreadyDeleted_ThrowsDomainException()
    {
        var company = CreateValidCompany();
        company.Delete();

        var act = () => company.Delete();

        act.Should().Throw<DomainException>()
           .WithMessage("*already deleted*");
    }

    [Fact]
    public void CompleteSetup_WhenDeleted_ThrowsDomainException()
    {
        var company = CreateValidCompany();
        company.Delete();

        var act = () => company.CompleteSetup();

        act.Should().Throw<DomainException>()
           .WithMessage("*deleted*");
    }

    // ═══════════════════════════════════════════════
    // UPDATE TESTS
    // ═══════════════════════════════════════════════

    [Fact]
    public void UpdateDetails_WithValidData_UpdatesFieldsAndSetsUpdatedAt()
    {
        var company = CreateValidCompany();

        company.UpdateDetails("NewName Ltd", "NewBrand", "Finance", CompanySize.Medium);

        company.Name.Should().Be("NewName Ltd");
        company.TradeName.Should().Be("NewBrand");
        company.Industry.Should().Be("Finance");
        company.Size.Should().Be(CompanySize.Medium);
        company.UpdatedAt.Should().NotBeNull();
        company.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateDetails_WhenDeleted_ThrowsDomainException()
    {
        var company = CreateValidCompany();
        company.Delete();

        var act = () => company.UpdateDetails("New", "New", "IT", CompanySize.Small);

        act.Should().Throw<DomainException>()
           .WithMessage("*deleted*");
    }

    // ═══════════════════════════════════════════════
    // DOMAIN EVENTS TESTS
    // ═══════════════════════════════════════════════

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        var company = CreateValidCompany();
        company.DomainEvents.Should().HaveCount(1);  // CompanyCreatedEvent

        company.ClearDomainEvents();

        company.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void MultipleOperations_AccumulateDomainEvents()
    {
        var company = CreateValidCompany();
        company.CompleteSetup();
        company.UpdateDetails("Updated Name", "Brand", "IT", CompanySize.Large);

        // Should have: CompanyCreatedEvent, CompanySetupCompletedEvent, CompanyUpdatedEvent
        company.DomainEvents.Should().HaveCount(3);
        company.DomainEvents.Should().ContainItemsAssignableTo<CompanyCreatedEvent>();
        company.DomainEvents.Should().ContainItemsAssignableTo<CompanySetupCompletedEvent>();
        company.DomainEvents.Should().ContainItemsAssignableTo<CompanyUpdatedEvent>();
    }
}