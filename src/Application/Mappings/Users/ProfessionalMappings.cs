using Application.DTO.Auth;
using Application.DTO.Common;
using Application.DTO.Users;
using Domain.Entities;
using Domain.Entities.Profiles;
using Domain.ValueObjects;

namespace Application.Mappings.Users;

public static class ProfessionalMappings
{
    public static User ToUser(this RegisterProfessionalRequest dto, string passwordHash)
        => new(
            name: new Name(dto.Name.FirstName, dto.Name.LastName),
            email: new Email(dto.Email),
            passwordHash: passwordHash,
            role: dto.Role,
            phone: dto.Phone,
            cpf: dto.Cpf
        );

    public static ProfessionalProfile ToProfile(this RegisterProfessionalRequest dto, Guid userId)
        => new(
            userId: userId,
            documentNumber: dto.DocumentNumber,
            speciality: dto.Speciality,
            address: new Address(
                number: dto.Address.Number,
                street: dto.Address.Street,
                city: dto.Address.City,
                state: dto.Address.State,
                country: dto.Address.Country,
                zipCode: dto.Address.ZipCode
            )
        );
}
