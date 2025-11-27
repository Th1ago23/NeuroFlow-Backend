using Domain.Enums;

namespace Domain.Entities;

public class Appointment
{
    private Appointment() { }

    public Appointment(Guid professionalUserId, Guid patientId, DateTime scheduledDateTime)
    {
        Id = Guid.NewGuid();
        ProfessionalUserId = professionalUserId;
        PatientId = patientId;
        ScheduledDateTime = scheduledDateTime;
        Status = AppointmentStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
    }
    public Guid Id { get; private set; }
    public Guid ProfessionalUserId { get; private set; }
    public User Professional { get; private set; } = null!;

    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;

    public DateTime ScheduledDateTime { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public AppointmentStatus Status { get; private set; }

    public void Cancel() => Status = AppointmentStatus.Cancelled;
    public void Complete() => Status = AppointmentStatus.Completed;
}


