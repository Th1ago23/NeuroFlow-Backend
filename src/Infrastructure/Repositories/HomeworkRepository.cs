using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class HomeworkRepository : BaseRepository<Homework>, IHomeworkRepository
{
    public HomeworkRepository(NeuroFlowContext context) : base(context) { }

    public async Task<IEnumerable<Homework>> GetByPatientAsync(Guid patientId)
    {
        return await _dbSet
            .Where(h => h.PatientId == patientId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Homework>> GetByProfessionalAsync(Guid professionalUserId)
    {
        return await _dbSet
            .Where(h => h.ProfessionalUserId == professionalUserId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
    }

    public async Task<Homework?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(h => h.Id == id);
    }
}
