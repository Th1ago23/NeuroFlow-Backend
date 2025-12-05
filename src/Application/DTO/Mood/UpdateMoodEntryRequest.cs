using Domain.Enums;

namespace Application.DTO.Mood
{
    public sealed record UpdateMoodEntryRequest(
        Guid Id,
        MoodLevel Level,
        string? Note
    );
}
