using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MoodEntryRepository : BaseRepository<MoodEntry>, IMoodEntryRepository
{
    public MoodEntryRepository(NeuroFlowContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MoodEntry>> GetByPatientAsync(Guid patientId)
    {
        return await _dbSet
            .Where(m => m.PatientId == patientId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }
    public async Task<IEnumerable<MoodEntry>> GetRecentAsync(Guid patientId, int limit = 20, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(m => m.PatientId == patientId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<MoodEntry>> GetByDateRangeAsync(Guid patientId, DateTime start, DateTime end, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(m => m.PatientId == patientId && m.CreatedAt >= start && m.CreatedAt <= end)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(ct);
    }
}
