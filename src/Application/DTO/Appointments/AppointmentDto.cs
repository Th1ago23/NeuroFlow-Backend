using Domain.Enums;

namespace Application.DTO.Appointments
{
    public sealed record AppointmentDto(
        Guid Id,
        DateTime ScheduledDateTime,
        AppointmentStatus Status
    );
}
