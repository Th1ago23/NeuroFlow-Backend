using Application.DTO.Common;
using Domain.Enums;

namespace Application.DTO.Users
{
    public sealed record UserSummaryDto(
        Guid Id,
        NameDto Name,
        string Email,
        UserRole Role,
        bool IsActive,
        DateTime CreatedAt
    );
}
