using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IMoodEntryRepository : IGenericRepository<MoodEntry>
{
    Task<IEnumerable<MoodEntry>> GetByPatientAsync(Guid patientId);
    Task<IEnumerable<MoodEntry>> GetRecentAsync(Guid patientId, int limit = 20, CancellationToken ct = default);
    Task<IEnumerable<MoodEntry>> GetByDateRangeAsync(Guid patientId, DateTime start, DateTime end, CancellationToken ct = default);
}
