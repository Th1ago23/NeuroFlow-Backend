using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetUpcomingByProfessionalAsync(Guid professionalUserId);
        Task<IEnumerable<Appointment>> GetUpcomingByPatientAsync(Guid patientId);
    }
}
