using Application.DTO.Common;
using Application.DTO.Users;
using Domain.Entities;

namespace Application.Mappings.Users
{
    public static class UserSummaryMappings
    {
        public static UserSummaryDto ToSummaryDto(this User user)
        => new(
            Id: user.Id,
            Name: new NameDto(user.Name.FirstName, user.Name.LastName),
            Email: user.Email.Address,
            Role: user.Role,
            IsActive: user.IsActive,
            CreatedAt: user.CreatedAt
        );
    }
}
