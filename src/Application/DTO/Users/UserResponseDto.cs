namespace Application.DTO.Users;
public record UserResponseDto(
    Guid Id,
    string Name,
    string Email,
    string? Phone,
    string Role,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastLoginAt
);
