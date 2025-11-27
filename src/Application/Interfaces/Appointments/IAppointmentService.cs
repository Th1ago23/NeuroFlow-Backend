using Application.DTO.Appointments;

namespace Application.Interfaces.Appointments;

public interface IAppointmentService
{
    Task<Guid> CreateAsync(CreateAppointmentRequest request);
    Task<IEnumerable<AppointmentDto>> GetUpcomingByProfessionalAsync(Guid professionalId);
    Task<IEnumerable<AppointmentDto>> GetUpcomingByPatientAsync(Guid patientId);
    Task CancelAsync(Guid appointmentId);
    Task CompleteAsync(Guid appointmentId);
}
