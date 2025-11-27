using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(NeuroFlowContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Appointment>> GetUpcomingByProfessionalAsync(Guid professionalId)
    {
        return await _dbSet
            .Where(a => a.ProfessionalUserId == professionalId &&
                        a.ScheduledDateTime >= DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetUpcomingByPatientAsync(Guid patientId)
    {
        return await _dbSet
            .Where(a => a.PatientId == patientId &&
                        a.ScheduledDateTime >= DateTime.UtcNow)
            .ToListAsync();
    }
}
