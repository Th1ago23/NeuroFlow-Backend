namespace Application.DTO.Mood;

public sealed record MoodStatsDto(
    int Count,
    double WeeklyAverage,
    double MonthlyAverage
);
