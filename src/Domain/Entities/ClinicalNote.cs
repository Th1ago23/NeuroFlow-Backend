namespace Domain.Entities;

public class ClinicalNote
{
    public ClinicalNote() { }

    public ClinicalNote(Guid patientId, string note)
    {
        Id = Guid.NewGuid();
        PatientId = patientId;
        Note = note;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Note { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;

    public void Update(string newNote)
    {
        Note = newNote;
    }
}

