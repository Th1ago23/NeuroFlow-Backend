using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ClinicalAssessmentRepository : BaseRepository<ClinicalAssessment>, IClinicalAssessmentRepository
{
    public ClinicalAssessmentRepository(NeuroFlowContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ClinicalAssessment>> GetByPatientAsync(Guid patientId)
    {
        return await _dbSet
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }
}
