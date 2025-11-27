namespace Domain.ValueObjects;

public class Email
{
    private Email() { }

    public Email(string address)
    {
        Address = address;
    }

    public string Address { get; private set; }
}
