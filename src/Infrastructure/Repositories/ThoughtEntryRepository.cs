using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ThoughtEntryRepository : BaseRepository<ThoughtEntry>, IThoughtEntryRepository
{
    public ThoughtEntryRepository(NeuroFlowContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ThoughtEntry>> GetByPatientAsync(Guid patientId)
    {
        return await _dbSet
            .Include(t => t.Category)
            .Where(t => t.PatientId == patientId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}
