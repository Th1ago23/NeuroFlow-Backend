namespace Application.DTO.Mood
{
    public sealed record UpdateMoodEntryRequest(
        Guid Id,
        int Level,
        string? Note
    );
}
