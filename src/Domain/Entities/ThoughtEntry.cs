namespace Domain.Entities;

public class ThoughtEntry
{
    private ThoughtEntry() { }

    public ThoughtEntry(
        Guid patientId,
        int categoryId,
        string? notes = null,
        bool isVisibleToProfessional = true)
    {
        Id = Guid.NewGuid();
        PatientId = patientId;
        CategoryId = categoryId;
        Notes = notes;
        IsVisibleToProfessional = isVisibleToProfessional;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;

    public int CategoryId { get; private set; }
    public ThoughtCategory Category { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    public string? Notes { get; private set; }
    public bool IsVisibleToProfessional { get; private set; }
}

