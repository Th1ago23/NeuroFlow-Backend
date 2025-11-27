using Application.DTO.Common;
using Application.DTO.Users;
using Domain.Entities.Profiles;

namespace Application.Mappings.Users;

public static class ProfessionalProfileMappings
{
    public static ProfessionalProfileDto ToDto(this ProfessionalProfile p)
        => new(
            DocumentNumber: p.DocumentNumber,
            Speciality: p.Speciality,
            Address: new AddressDto(
                Number: p.Address.Number,
                Street: p.Address.Street,
                City: p.Address.City,
                State: p.Address.State,
                Country: p.Address.Country,
                ZipCode: p.Address.ZipCode
            ),
            IsVerified: p.IsVerified,
            IsPremium: p.IsPremium,
            DocumentFileUrl: p.DocumentFileUrl
        );
}
