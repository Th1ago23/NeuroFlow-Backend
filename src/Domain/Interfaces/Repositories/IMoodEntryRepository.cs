using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IMoodEntryRepository : IGenericRepository<MoodEntry>
{
    Task<IEnumerable<MoodEntry>> GetByPatientAsync(Guid patientId);
}
