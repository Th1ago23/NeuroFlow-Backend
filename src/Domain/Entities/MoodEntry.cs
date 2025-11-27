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

    public MoodLevel Level { get; private set; }      // enum de humor
    public string? Note { get; private set; }         // opcional
    public DateTime CreatedAt { get; private set; }   // timestamp exato da entrada
}
