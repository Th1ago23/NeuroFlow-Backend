using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PatientRepository : BaseRepository<Patient>, IPatientRepository
{
    public PatientRepository(NeuroFlowContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Patient>> GetByProfessionalIdAsync(Guid professionalUserId)
    {
        return await _dbSet
            .Include(p => p.MoodEntries)
            .Include(p => p.ThoughtEntries)
            .Include(p => p.Notes)
            .Include(p => p.Assessments)
            .Where(p => p.OwnerProfessionalId == professionalUserId)
            .ToListAsync();
    }
    public async Task<Patient> GetByUserId(Guid userId, CancellationToken ct)
    {
        return await _dbSet
            .Include(i => i.MoodEntries)
            .Include(i=> i.ThoughtEntries)
            .Include(i => i.Address)
            .Include(i => i.Assessments)
            .Include(i => i.Notes)
            .FirstOrDefaultAsync(i => i.UserAccountId == userId,ct);
    }
}
