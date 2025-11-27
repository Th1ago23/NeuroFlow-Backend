namespace Domain.Entities.Security;

public sealed record JwtTokenResult(
    string AccessToken,
    DateTime ExpiresAt,
    string Jti
);
