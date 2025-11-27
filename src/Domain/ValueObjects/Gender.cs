using Domain.Enums;

namespace Domain.ValueObjects;

public class Gender
{
    public GenderType Type { get; }
    public string? CustomValue { get; }

    private Gender() { }

    public Gender(GenderType type, string? customValue = null)
    {
        if (type != GenderType.Other && customValue is not null)
            throw new ArgumentException("CustomValue só pode ser preenchido se o gênero for 'Other'.");

        if (type == GenderType.Other && string.IsNullOrWhiteSpace(customValue))
            throw new ArgumentException("Para gênero 'Other', é necessário uma descrição.");

        Type = type;
        CustomValue = customValue;
    }

    public override string ToString()
    {
        return Type == GenderType.Other ? CustomValue! : Type.ToString();
    }
}
