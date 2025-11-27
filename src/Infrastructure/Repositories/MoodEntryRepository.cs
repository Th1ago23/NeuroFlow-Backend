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
}
