namespace Application.DTO.Auth;

public sealed record LoginRequest(
    string Email,
    string Password
);
