using Domain.Enums;

namespace Domain.Entities;

public class MoodEntry
{
    private MoodEntry() { }

    public MoodEntry(
        Guid patientId,
        MoodLevel level,
        string? note = null,
        DateTime? createdAt = null)
    {
        Id = Guid.NewGuid();
        PatientId = patientId;
        Level = level;
        Note = note;
        CreatedAt = createdAt ?? DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;

    public MoodLevel Level { get; private set; }
    public string? Note { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public void Update(MoodLevel level, string? note)
    {
        Level = level;
        Note = note;
    }
}
