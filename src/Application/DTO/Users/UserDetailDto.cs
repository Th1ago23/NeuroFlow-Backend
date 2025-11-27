using Application.DTO.Common;
using Domain.Enums;

namespace Application.DTO.Users
{
    public sealed record UserDetailDto(
        Guid Id,
        NameDto Name,
        string Email,
        UserRole Role,
        bool IsActive,
        DateTime CreatedAt,
        ProfessionalProfileDto? ProfessionalProfile
    );
}
