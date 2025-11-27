using Application.DTO.Common;

namespace Application.DTO.Users;
public record UpdateUserDto(
    NameDto? Name,
    string? Email,
    string? Phone
);
