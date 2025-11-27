using Application.DTO.Common;

namespace Application.DTO.Auth
{
    public sealed record RegisterPatientRequest(
        Guid InviteToken,
        NameDto Name,
        string Email,
        string Password,
        string? Phone,
        GenderDto Gender,
        DateOnly BirthDate,
        AddressDto? Address
    );
}
