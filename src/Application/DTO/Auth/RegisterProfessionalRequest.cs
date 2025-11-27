using Application.DTO.Common;
using Domain.Enums;

namespace Application.DTO.Auth
{
    public sealed record RegisterProfessionalRequest(
        NameDto Name,
        string Email,
        string Password,
        string? Phone,
        string? Cpf,
        string DocumentNumber,
        string Speciality,
        AddressDto Address,
        UserRole Role
    );
}
