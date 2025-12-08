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
    public async Task<ProfessionalProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId, ct);
    }

    public async Task<ProfessionalProfile?> GetBySubscriptionIdAsync(string subscriptionId, CancellationToken ct)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.SubscriptionId == subscriptionId, ct);
    }
}
