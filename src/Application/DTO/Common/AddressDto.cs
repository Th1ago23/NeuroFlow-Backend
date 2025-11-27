namespace Application.DTO.Common;

public sealed record AddressDto(
    int Number,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode
);
