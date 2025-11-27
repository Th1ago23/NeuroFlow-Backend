using Application.DTO.Common;
using Domain.ValueObjects;

namespace Application.Mappings.Common;

public static class GenderMappings
{
    public static GenderDto ToDto(this Gender gender)
        => new(gender.Type, gender.CustomValue);

    public static Gender ToValueObject(this GenderDto dto)
        => new(dto.Type, dto.CustomValue);
}
