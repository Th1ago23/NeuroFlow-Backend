namespace Domain.Entities;

public class PatientInvite
{
    private PatientInvite() { }

    public PatientInvite(Guid professionalUserId, Guid token, DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        ProfessionalUserId = professionalUserId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        IsUsed = false;
    }

    public Guid Id { get; private set; }
    public Guid ProfessionalUserId { get; private set; }
    public Guid Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public void MarkAsUsed() => IsUsed = true;
}
