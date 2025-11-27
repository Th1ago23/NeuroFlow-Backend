using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PatientInviteRepository : BaseRepository<PatientInvite>, IPatientInviteRepository
{
    public PatientInviteRepository(NeuroFlowContext context)
        : base(context) { }

    public async Task<PatientInvite?> GetActiveByTokenAsync(Guid token, CancellationToken ct = default)
    {
        return await _dbSet.FirstOrDefaultAsync(
            x => x.Token == token && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow,
            ct);
    }
}
