using Application.DTO.Mood;
using Domain.Entities;

namespace Application.Mappings.Mood;

public static class MoodMappings
{
    public static MoodEntryDto ToDto(this MoodEntry entry)
        => new(
            Id: entry.Id,
            Level: entry.Level,
            Note: entry.Note,
            CreatedAt: entry.CreatedAt
        );

    public static MoodEntry ToEntity(this CreateMoodEntryRequest request)
        => new(
            patientId: request.PatientId,
            level: request.Level,
            note: request.Note
        );

    public static MoodStatsDto ToStatsDto(
        this IEnumerable<MoodEntry> entries,
        DateTime? referenceDate = null)
    {
        var list = entries.ToList();

        var totalCount = list.Count;

        var now = referenceDate ?? DateTime.UtcNow;
        var weekStart = now.AddDays(-7);
        var monthStart = now.AddDays(-30);

        var weekCount = list.Count(e => e.CreatedAt >= weekStart && e.CreatedAt <= now);
        var monthCount = list.Count(e => e.CreatedAt >= monthStart && e.CreatedAt <= now);

        var weeklyAverage = weekCount / 7d;
        var monthlyAverage = monthCount / 30d;

        return new MoodStatsDto(
            Count: totalCount,
            WeeklyAverage: weeklyAverage,
            MonthlyAverage: monthlyAverage
        );
    }
}
