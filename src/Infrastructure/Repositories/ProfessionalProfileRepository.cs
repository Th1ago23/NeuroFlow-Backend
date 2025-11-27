using Domain.Entities.Profiles;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProfessionalProfileRepository : BaseRepository<ProfessionalProfile>, IProfessionalProfileRepository
{
    public ProfessionalProfileRepository(NeuroFlowContext context) : base(context)
    {
    }

    public async Task<ProfessionalProfile?> GetByDocumentAsync(string documentNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.DocumentNumber == documentNumber);
    }
}
