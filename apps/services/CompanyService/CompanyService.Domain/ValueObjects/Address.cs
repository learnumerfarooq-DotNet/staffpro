// ─────────────────────────────────────────────────────────────────────────
// Address.cs — Address Value Object (UPGRADED)
//
// CHANGES FROM WEEK 1 (Day 3):
//   ✅ Equality is now CASE-INSENSITIVE
//      "London" == "london" → true  (user typos shouldn't cause duplicates)
//   ✅ Proper null-safe == and != operator overloads
//      Week 1 used EqualityComparer<Address>.Default which boxes the value
//   ✅ GetHashCode is consistent with case-insensitive Equals
//      Required rule: if a.Equals(b) == true then a.GetHashCode() == b.GetHashCode()
//   ✅ WithX() copy methods (immutable update pattern)
//      Instead of: new Address(newStreet, addr.City, addr.State, ...)
//      You write:  addr.WithStreet("456 New Road")
//
// VALUE OBJECT RULES (reminder):
//   1. Immutable — no setters, init-only or private readonly
//   2. Equality by VALUE — two Address objects with the same fields are equal
//   3. No own identity — Address has no Id; it belongs to Company
//   4. Use factory method — private constructor + public static Create()
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Exceptions;

namespace CompanyService.Domain.ValueObjects;

/// <summary>
/// Represents a physical postal address.
/// Immutable — once created, the only way to "change" an address is to
/// create a new one (or use the WithX() helper methods below).
/// </summary>
public sealed class Address : IEquatable<Address>
{
    // ─────────────────────────────────────────────────
    // Properties — init-only (cannot be changed after construction)
    // ─────────────────────────────────────────────────

    /// <summary>Building number and street name. E.g. "24 Baker Street"</summary>
    public string Street { get; init; }

    /// <summary>Optional apartment, suite, or floor. E.g. "Suite 4B"</summary>
    public string? ApartmentSuite { get; init; }

    /// <summary>City or town. E.g. "Manchester"</summary>
    public string City { get; init; }

    /// <summary>State, province, or region. E.g. "England" or "Punjab"</summary>
    public string State { get; init; }

    /// <summary>Postal / ZIP code. Stored as string (may contain letters: "SW1 1AA")</summary>
    public string PostalCode { get; init; }

    /// <summary>Country. E.g. "United Kingdom", "Pakistan"</summary>
    public string Country { get; init; }

    // ─────────────────────────────────────────────────
    // Private Constructor
    // ─────────────────────────────────────────────────

    private Address(
        string street,
        string city,
        string state,
        string postalCode,
        string country,
        string? apartmentSuite = null)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
        ApartmentSuite = apartmentSuite;
    }

    // ─────────────────────────────────────────────────
    // FACTORY METHOD — validates and creates
    // ─────────────────────────────────────────────────

    /// <summary>
    /// Creates a validated Address. Throws DomainException if any required
    /// field is missing. Trims all inputs automatically.
    /// </summary>
    public static Address Create(
        string street,
        string city,
        string state,
        string postalCode,
        string country,
        string? apartmentSuite = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Street address is required.");
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City is required.");
        if (string.IsNullOrWhiteSpace(country))
            throw new DomainException("Country is required.");
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new DomainException("Postal code is required.");

        return new Address(
            street.Trim(),
            city.Trim(),
            state.Trim(),
            postalCode.Trim(),
            country.Trim(),
            apartmentSuite?.Trim());
    }

    // ─────────────────────────────────────────────────
    // IMMUTABLE COPY METHODS — "with" pattern
    // ─────────────────────────────────────────────────
    //
    // PROBLEM: Address is immutable, so you can't do addr.City = "Leeds".
    // SOLUTION: "With" methods return a NEW address with one field changed.
    //
    // Usage in Company:
    //   company.UpdateHeadOffice(company.HeadOfficeAddress.WithCity("Leeds"));
    //
    // The original address is UNCHANGED — a new object is returned.

    /// <summary>Returns a new Address with the street changed.</summary>
    public Address WithStreet(string newStreet)
        => Create(newStreet, City, State, PostalCode, Country, ApartmentSuite);

    /// <summary>Returns a new Address with the city changed.</summary>
    public Address WithCity(string newCity)
        => Create(Street, newCity, State, PostalCode, Country, ApartmentSuite);

    /// <summary>Returns a new Address with the country changed.</summary>
    public Address WithCountry(string newCountry)
        => Create(Street, City, State, PostalCode, newCountry, ApartmentSuite);

    /// <summary>Returns a new Address with the postal code changed.</summary>
    public Address WithPostalCode(string newPostalCode)
        => Create(Street, City, State, newPostalCode, Country, ApartmentSuite);

    // ─────────────────────────────────────────────────
    // VALUE EQUALITY — case-insensitive
    // ─────────────────────────────────────────────────
    //
    // WHY CASE-INSENSITIVE?
    //   One user types: "London", another types: "london"
    //   They are the same city — we don't want to treat them as different addresses.
    //   StringComparison.OrdinalIgnoreCase handles this cleanly.

    /// <summary>
    /// Two addresses are equal if all their field values match (case-insensitive).
    /// </summary>
    public bool Equals(Address? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return string.Equals(Street, other.Street, StringComparison.OrdinalIgnoreCase)
            && string.Equals(City, other.City, StringComparison.OrdinalIgnoreCase)
            && string.Equals(State, other.State, StringComparison.OrdinalIgnoreCase)
            && string.Equals(PostalCode, other.PostalCode, StringComparison.OrdinalIgnoreCase)
            && string.Equals(Country, other.Country, StringComparison.OrdinalIgnoreCase)
            && string.Equals(ApartmentSuite, other.ApartmentSuite, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as Address);

    /// <summary>
    /// Hash code MUST be consistent with Equals().
    /// THE RULE: if a.Equals(b) == true, then a.GetHashCode() == b.GetHashCode().
    /// ToUpperInvariant() makes hash case-insensitive to match our Equals().
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(
        Street.ToUpperInvariant(),
        City.ToUpperInvariant(),
        State.ToUpperInvariant(),
        PostalCode.ToUpperInvariant(),
        Country.ToUpperInvariant(),
        ApartmentSuite?.ToUpperInvariant());

    // ─────────────────────────────────────────────────
    // OPERATOR OVERLOADS — enables addr1 == addr2 syntax
    // ─────────────────────────────────────────────────
    //
    // Without these, "addr1 == addr2" checks REFERENCE equality (same object
    // in memory), not VALUE equality. We always want value equality for VOs.
    //
    // Null handling:
    //   null == null  → true
    //   null == addr  → false
    //   addr == null  → false

    public static bool operator ==(Address? left, Address? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Address? left, Address? right)
        => !(left == right);

    // ─────────────────────────────────────────────────
    // Display
    // ─────────────────────────────────────────────────

    /// <summary>Human-readable address string for logging and UI display.</summary>
    public override string ToString()
    {
        var apt = ApartmentSuite is not null ? $", {ApartmentSuite}" : string.Empty;
        return $"{Street}{apt}, {City}, {State} {PostalCode}, {Country}";
    }
}