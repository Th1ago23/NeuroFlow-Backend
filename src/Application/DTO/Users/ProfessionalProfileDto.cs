using Application.DTO.Common;

namespace Application.DTO.Users;

public sealed record ProfessionalProfileDto(
    string DocumentNumber,
    string Speciality,
    AddressDto Address,
    bool IsVerified,
    bool IsPremium,
    string? DocumentFileUrl
);
