namespace Application.DTO.Appointments;

public sealed record CreateAppointmentRequest(
    Guid ProfessionalUserId,
    Guid PatientId,
    DateTime ScheduledDateTime
);
