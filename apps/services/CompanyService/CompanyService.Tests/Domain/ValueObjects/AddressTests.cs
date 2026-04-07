// ─────────────────────────────────────────────────────────────────────────
// AddressTests.cs — Unit Tests for the Address Value Object
//
// Focuses on:
//   1. Creation validation — missing fields throw the right exceptions
//   2. Value equality — case-insensitive, == and != operators
//   3. Immutability — With*() returns new objects, originals unchanged
//   4. GetHashCode consistency — equal addresses must have equal hash codes
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Exceptions;
using CompanyService.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CompanyService.Tests.Domain.ValueObjects;

public class AddressTests
{
    // ─────────────────────────────────────────────────
    // Shared helper
    // ─────────────────────────────────────────────────

    private static Address LondonAddress() =>
        Address.Create("24 Baker Street", "London", "England", "W1U 3BW", "United Kingdom");

    // ═══════════════════════════════════════════════
    // CREATION TESTS
    // ═══════════════════════════════════════════════

    [Fact]
    public void Create_WithValidData_ReturnsAddressWithCorrectValues()
    {
        var address = Address.Create("24 Baker Street", "London", "England", "W1U 3BW", "UK");

        address.Street.Should().Be("24 Baker Street");
        address.City.Should().Be("London");
        address.State.Should().Be("England");
        address.PostalCode.Should().Be("W1U 3BW");
        address.Country.Should().Be("UK");
        address.ApartmentSuite.Should().BeNull();
    }

    [Fact]
    public void Create_WithApartmentSuite_SetsItCorrectly()
    {
        var address = Address.Create("24 Baker Street", "London", "England", "W1U 3BW", "UK", "Suite 4B");

        address.ApartmentSuite.Should().Be("Suite 4B");
    }

    [Fact]
    public void Create_TrimsAllFields()
    {
        var address = Address.Create("  24 Baker Street  ", "  London  ", "  England  ", "  W1U 3BW  ", "  UK  ");

        address.Street.Should().Be("24 Baker Street");
        address.City.Should().Be("London");
        address.Country.Should().Be("UK");
    }

    [Theory]
    [InlineData("", "London", "England", "W1U 3BW", "UK")]  // empty street
    [InlineData("24 Baker", "", "England", "W1U 3BW", "UK")]  // empty city
    [InlineData("24 Baker", "London", "England", "", "UK")]  // empty postal
    [InlineData("24 Baker", "London", "England", "W1U 3BW", "")]   // empty country
    [InlineData(null, "London", "England", "W1U 3BW", "UK")]        // null street
    public void Create_WithMissingRequiredField_ThrowsDomainException(
        string? street, string city, string state, string postalCode, string country)
    {
        var act = () => Address.Create(street!, city, state, postalCode, country);

        act.Should().Throw<DomainException>();
    }

    // ═══════════════════════════════════════════════
    // VALUE EQUALITY TESTS
    // ═══════════════════════════════════════════════

    [Fact]
    public void TwoAddresses_WithSameValues_AreEqual()
    {
        var addr1 = Address.Create("24 Baker Street", "London", "England", "W1U 3BW", "UK");
        var addr2 = Address.Create("24 Baker Street", "London", "England", "W1U 3BW", "UK");

        addr1.Should().Be(addr2);
        (addr1 == addr2).Should().BeTrue();
        (addr1 != addr2).Should().BeFalse();
    }

    [Fact]
    public void TwoAddresses_WithSameValuesDifferentCase_AreEqual()
    {
        // Key Week 2 upgrade: case-insensitive equality
        var addr1 = Address.Create("24 Baker Street", "London", "England", "W1U 3BW", "UK");
        var addr2 = Address.Create("24 BAKER STREET", "LONDON", "england", "w1u 3bw", "uk");

        addr1.Should().Be(addr2);
        (addr1 == addr2).Should().BeTrue();
    }

    [Fact]
    public void TwoAddresses_WithDifferentStreet_AreNotEqual()
    {
        var addr1 = Address.Create("24 Baker Street", "London", "England", "W1U 3BW", "UK");
        var addr2 = Address.Create("99 Other Road", "London", "England", "W1U 3BW", "UK");

        addr1.Should().NotBe(addr2);
        (addr1 == addr2).Should().BeFalse();
        (addr1 != addr2).Should().BeTrue();
    }

    [Fact]
    public void TwoAddresses_WithDifferentCity_AreNotEqual()
    {
        var addr1 = Address.Create("24 Baker Street", "London", "England", "W1U 3BW", "UK");
        var addr2 = Address.Create("24 Baker Street", "Manchester", "England", "M1 1AA", "UK");

        addr1.Should().NotBe(addr2);
    }

    [Fact]
    public void EqualAddresses_HaveSameHashCode()
    {
        // Critical rule: if Equals() == true, GetHashCode() must be equal
        var addr1 = Address.Create("24 Baker Street", "London", "England", "W1U 3BW", "UK");
        var addr2 = Address.Create("24 BAKER STREET", "london", "ENGLAND", "W1U 3BW", "uk");

        addr1.GetHashCode().Should().Be(addr2.GetHashCode());
    }

    [Fact]
    public void Address_ComparedWithNull_ReturnsFalse()
    {
        var addr = LondonAddress();
        Address? nullAddr = null;

        addr.Equals(nullAddr).Should().BeFalse();
        (addr == nullAddr).Should().BeFalse();
        (nullAddr == addr).Should().BeFalse();
    }

    [Fact]
    public void TwoNullAddresses_AreEqual()
    {
        Address? a = null;
        Address? b = null;

        (a == b).Should().BeTrue();
    }

    // ═══════════════════════════════════════════════
    // IMMUTABILITY (With* COPY METHODS) TESTS
    // ═══════════════════════════════════════════════

    [Fact]
    public void WithCity_ReturnsNewAddressWithUpdatedCity_OriginalUnchanged()
    {
        var original = LondonAddress();

        var updated = original.WithCity("Manchester");

        // New address has updated city
        updated.City.Should().Be("Manchester");
        // Original is UNCHANGED (immutability guaranteed)
        original.City.Should().Be("London");
    }

    [Fact]
    public void WithStreet_ReturnsNewAddressWithUpdatedStreet()
    {
        var original = LondonAddress();

        var updated = original.WithStreet("99 New Road");

        updated.Street.Should().Be("99 New Road");
        original.Street.Should().Be("24 Baker Street");   // unchanged
    }

    [Fact]
    public void WithPostalCode_ReturnsNewAddress_WithAllOtherFieldsCopied()
    {
        var original = LondonAddress();

        var updated = original.WithPostalCode("EC1 2AA");

        updated.PostalCode.Should().Be("EC1 2AA");
        updated.Street.Should().Be(original.Street);   // copied from original
        updated.City.Should().Be(original.City);       // copied from original
        updated.Country.Should().Be(original.Country); // copied from original
    }

    // ═══════════════════════════════════════════════
    // TOSTRING TEST
    // ═══════════════════════════════════════════════

    [Fact]
    public void ToString_WithoutApartmentSuite_FormatsCorrectly()
    {
        var addr = Address.Create("24 Baker Street", "London", "England", "W1U 3BW", "UK");

        addr.ToString().Should().Be("24 Baker Street, London, England W1U 3BW, UK");
    }

    [Fact]
    public void ToString_WithApartmentSuite_IncludesItInOutput()
    {
        var addr = Address.Create("24 Baker Street", "London", "England", "W1U 3BW", "UK", "Suite 4B");

        addr.ToString().Should().Be("24 Baker Street, Suite 4B, London, England W1U 3BW, UK");
    }
}