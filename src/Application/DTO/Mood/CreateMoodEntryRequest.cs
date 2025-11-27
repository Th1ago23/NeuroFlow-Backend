using Domain.Enums;

namespace Application.DTO.Mood;

public sealed record CreateMoodEntryRequest(
    Guid PatientId,
    MoodLevel Level,
    string? Note
);
