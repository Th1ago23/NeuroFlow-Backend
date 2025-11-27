using Application.DTO.Users;

namespace Application.DTO.Auth;

public sealed record LoginResponse(
    string AccessToken,
    DateTime ExpiresAt,
    UserSummaryDto User
);
