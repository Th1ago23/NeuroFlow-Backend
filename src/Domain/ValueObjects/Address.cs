
namespace Domain.ValueObjects;

public class Address
{

    public int Number { get; }
    public string Street { get; } = string.Empty;
    public string City { get; } = string.Empty;
    public string State { get; } = string.Empty;
    public string Country { get;  } = string.Empty;
    public string ZipCode { get; } = string.Empty;

    public Address(int number, string street, string city, string state, string country, string zipCode)
    {
        Number = number;
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        State = state ?? throw new ArgumentNullException(nameof(state));
        Country = country ?? throw new ArgumentNullException(nameof(country));
        ZipCode = zipCode ?? throw new ArgumentNullException(nameof(zipCode));
    }

    public Address()
    {
    }
}
