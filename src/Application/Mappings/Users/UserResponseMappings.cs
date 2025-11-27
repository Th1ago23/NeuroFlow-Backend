using Application.DTO.Users;
using Domain.Entities;

namespace Application.Mappings.Users;

public static class UserResponseMappings
{
    public static UserResponseDto ToResponse(this User user)
        => new(
            Id: user.Id,
            Name: $"{user.Name.FirstName} {user.Name.LastName}",
            Email: user.Email.Address,
            Phone: user.Phone,
            Role: user.Role.ToString(),
            IsActive: user.IsActive,
            CreatedAt: user.CreatedAt,
            LastLoginAt: user.LastLoginAt
        );
}
