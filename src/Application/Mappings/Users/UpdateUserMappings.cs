using Application.DTO.Users;
using Application.DTO.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Mappings.Users;

public static class UpdateUserMappings
{
    public static void ApplyUpdates(this User user, UpdateUserDto dto)
    {
        if (dto.Name is not null)
        {
            user.ChangeName(new Name(dto.Name.FirstName, dto.Name.LastName));
        }

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            user.ChangeEmail(new Email(dto.Email));
        }

        if (!string.IsNullOrWhiteSpace(dto.Phone))
        {
            user.ChangePhone(dto.Phone);
        }
    }
}
