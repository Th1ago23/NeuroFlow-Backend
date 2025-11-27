using Application.DTO.Common;
using Domain.ValueObjects;

namespace Application.Mappings.Common;

public static class NameMappings
{
    public static NameDto ToDto(this Name name)
        => new(name.FirstName, name.LastName);

    public static Name ToValueObject(this NameDto dto)
        => new(dto.FirstName, dto.LastName);
}
