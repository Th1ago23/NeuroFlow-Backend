using Application.DTO.Common;
using Domain.ValueObjects;

namespace Application.Mappings.Common;

public static class AddressMappings
{
    public static Address? ToValueObject(this AddressDto? dto)
        => dto is null
            ? null
            : new Address(
                number: dto.Number,
                street: dto.Street,
                city: dto.City,
                state: dto.State,
                country: dto.Country,
                zipCode: dto.ZipCode
            );

    public static AddressDto ToDto(this Address address)
        => new(
            Number: address.Number,
            Street: address.Street,
            City: address.City,
            State: address.State,
            Country: address.Country,
            ZipCode: address.ZipCode
        );
}
