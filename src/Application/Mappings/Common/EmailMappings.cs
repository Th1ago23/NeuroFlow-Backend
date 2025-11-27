using Application.DTO.Common;
using Domain.ValueObjects;

namespace Application.Mappings.Common;

public static class EmailMappings
{
    public static EmailDto ToDto(this Email email)
        => new(email.Address);

    public static Email ToValueObject(this EmailDto dto)
        => new(dto.Address);
}
