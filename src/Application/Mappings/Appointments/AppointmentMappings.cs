using Application.DTO.Appointments;
using Domain.Entities;

namespace Application.Mappings.Appointments;
public static class AppointmentMappings
{
    public static AppointmentDto ToDto(this Appointment appointment)
        => new(
            Id: appointment.Id,
            ScheduledDateTime: appointment.ScheduledDateTime,
            Status: appointment.Status
        );

    public static Appointment ToEntity(this CreateAppointmentRequest request)
        => new(
            professionalUserId: request.ProfessionalUserId,
            patientId: request.PatientId,
            scheduledDateTime: request.ScheduledDateTime
        );
}
