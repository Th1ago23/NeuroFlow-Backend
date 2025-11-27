namespace Domain.ValueObjects;

public class Name
{
    public string FirstName { get; }
    public string LastName { get; }

    private Name() { }

    public Name(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.");

        if (firstName.Length < 2)
            throw new ArgumentException("First name must contain at least 2 characters.");

        if (lastName.Length < 2)
            throw new ArgumentException("Last name must contain at least 2 characters.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public string FullName => $"{FirstName} {LastName}";

    public override bool Equals(object? obj)
    {
        if (obj is not Name other)
            return false;

        return FirstName == other.FirstName &&
               LastName == other.LastName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FirstName, LastName);
    }

    public override string ToString()
    {
        return FullName;
    }
}
