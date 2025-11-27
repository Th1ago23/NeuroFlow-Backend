using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ClinicalNoteRepository : BaseRepository<ClinicalNote>, IClinicalNoteRepository
{
    public ClinicalNoteRepository(NeuroFlowContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ClinicalNote>> GetByPatientAsync(Guid patientId)
    {
        return await _dbSet
            .Where(n => n.PatientId == patientId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }
}
