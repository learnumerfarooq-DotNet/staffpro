// ─────────────────────────────────────────────────────────────────────────
// Address.cs — Address Value Object
//
// Value Object rules:
//   1. Immutable (no setters, init-only properties)
//   2. Equality based on VALUES (not identity/Id)
//   3. No own identity (no Id property)
// ─────────────────────────────────────────────────────────────────────────

using CompanyService.Domain.Exceptions;

namespace CompanyService.Domain.ValueObjects;

/// <summary>
/// Represents a physical address. Immutable by design.
/// Two addresses are equal if all their field values are equal.
/// </summary>
public sealed class Address : IEquatable<Address>
{
    // ─────────────────────────────────────────────────
    // Properties — init-only (set only in constructor)
    // ─────────────────────────────────────────────────

    public string Street { get; init; }
    public string City { get; init; }
    public string State { get; init; }
    public string PostalCode { get; init; }
    public string Country { get; init; }
    public string? ApartmentSuite { get; init; }

    // ─────────────────────────────────────────────────
    // Private constructor
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
    // FACTORY METHOD
    // ─────────────────────────────────────────────────

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
    // VALUE EQUALITY — Two addresses are equal if values match
    // ─────────────────────────────────────────────────

    public bool Equals(Address? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Street == other.Street
            && City == other.City
            && State == other.State
            && PostalCode == other.PostalCode
            && Country == other.Country
            && ApartmentSuite == other.ApartmentSuite;
    }

    public override bool Equals(object? obj) => Equals(obj as Address);

    public override int GetHashCode() =>
        HashCode.Combine(Street, City, State, PostalCode, Country, ApartmentSuite);

    public static bool operator == (Address? left, Address? right) =>
        EqualityComparer<Address>.Default.Equals(left, right);

    public static bool operator != (Address? left, Address? right) => !(left == right);

    /// <summary>Format as a readable string</summary>
    public override string ToString()
    {
        var apt = ApartmentSuite is not null ? $", {ApartmentSuite}" : "";
        return $"{Street}{apt}, {City}, {State} {PostalCode}, {Country}";
    }
}