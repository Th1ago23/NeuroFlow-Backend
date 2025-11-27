using Domain.Enums;

namespace Application.DTO.Mood;

public sealed record MoodEntryDto(
    Guid Id,
    MoodLevel Level,
    string? Note,
    DateTime CreatedAt
);
