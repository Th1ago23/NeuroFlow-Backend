using Domain.Enums;

namespace Application.DTO.Common;

public sealed record GenderDto(
    GenderType Type,
    string? CustomValue
);
