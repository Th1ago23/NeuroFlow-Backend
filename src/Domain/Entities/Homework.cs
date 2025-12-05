namespace Domain.Entities;

public class Homework
{
    private Homework() { }

    public Homework(
        Guid professionalUserId,
        Guid patientId,
        string title,
        DateTime expirationTime,
        string? description = null)
    {
        Id = Guid.NewGuid();
        ProfessionalUserId = professionalUserId;
        PatientId = patientId;
        Title = title;
        Description = description;
        ExpirationTime = expirationTime;
        IsDone = false;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid ProfessionalUserId { get; private set; }
    public User Professional { get; private set; } = null!;

    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;

    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime ExpirationTime { get; private set; }
    public bool IsDone { get; private set; }
    public DateTime CreatedAt { get; private set; }


    public void MarkAsDone() => IsDone = true;

    public void Update(string title, string? description, DateTime expiration)
    {
        Title = title;
        Description = description;
        ExpirationTime = expiration;
    }
}
