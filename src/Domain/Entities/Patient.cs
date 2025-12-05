using Domain.ValueObjects;

namespace Domain.Entities;

public class Patient
{
    private Patient() { }

    public Patient(
        Name name,
        DateOnly birthDate,
        Gender gender,
        Guid ownerProfessionalId,
        Address? address = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        BirthDate = birthDate;
        Gender = gender;
        Address = address;

        OwnerProfessionalId = ownerProfessionalId;
        IsActive = true;

        ThoughtEntries = new List<ThoughtEntry>();
        MoodEntries = new List<MoodEntry>();
        Notes = new List<ClinicalNote>();
        Assessments = new List<ClinicalAssessment>();
    }

    public Guid Id { get; private set; }

    public Name Name { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public Address? Address { get; private set; }

    public Guid? UserAccountId { get; private set; }
    public User? UserAccount { get; private set; }

    public Guid? OwnerProfessionalId { get; private set; }
    public User OwnerProfessional { get; private set; } = null!;

    public bool IsActive { get; private set; }
    public DateTime? UnassignedAt { get; private set; }
    public List<ThoughtEntry> ThoughtEntries { get; private set; }
    public List<MoodEntry> MoodEntries { get; private set; }
    public List<ClinicalNote> Notes { get; private set; }
    public List<ClinicalAssessment> Assessments { get; private set; }
    public List<Homework> Homework { get; private set; } = new();

    public void LinkUserAccount(Guid userId)
    {
        UserAccountId = userId;
    }

    public void UpdateSensitiveData(Name name, Address? address)
    {
        Name = name;
        Address = address;
    }

    public void AssignProfessional(Guid professionalId)
    {
        OwnerProfessionalId = professionalId;
        UnassignedAt = null;
    }

    public void UnassignProfessional()
    {
        OwnerProfessionalId = null;
        UnassignedAt = DateTime.UtcNow;
    }

    public bool IsExpiredForDeletion()
    {
        return UnassignedAt.HasValue &&
               DateTime.UtcNow >= UnassignedAt.Value.AddYears(1);
    }

    public void Deactivate() => IsActive = false;
}
