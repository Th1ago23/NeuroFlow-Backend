using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IPatientInviteRepository : IGenericRepository<PatientInvite>
{
    Task<PatientInvite?> GetActiveByTokenAsync(Guid token,CancellationToken ct = default);
}
