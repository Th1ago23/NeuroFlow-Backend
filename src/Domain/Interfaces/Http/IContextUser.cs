namespace Domain.Interfaces.Http;

public interface IContextUser
{
    Guid UserId { get; }
    string? Email { get; }
    string? Role { get; }
}
