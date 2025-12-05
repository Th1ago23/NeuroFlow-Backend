namespace Domain.Entities;

public class ClinicalAssessment
{
    private ClinicalAssessment() { }

    public ClinicalAssessment(
        Guid professionalUserId,
        Guid patientId,
        DateTime date,
        string summary,
        string? diagnosis = null,
        string? recommendations = null)
    {
        Id = Guid.NewGuid();
        ProfessionalUserId = professionalUserId;
        PatientId = patientId;
        Date = date;
        Summary = summary;
        Diagnosis = diagnosis;
        Recommendations = recommendations;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid ProfessionalUserId { get; private set; }
    public User Professional { get; private set; } = null!;

    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;
    public DateTime Date { get; private set; }
    public string Summary { get; private set; }
    public string? Diagnosis { get; private set; }
    public string? Recommendations { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public void Update(string summary, string? diagnosis, string? recommendations)
    {
        Summary = summary;
        Diagnosis = diagnosis;
        Recommendations = recommendations;
    }
}

