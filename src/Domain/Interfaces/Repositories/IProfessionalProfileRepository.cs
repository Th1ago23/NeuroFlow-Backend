using Domain.Entities.Profiles;

namespace Domain.Interfaces.Repositories;

public interface IProfessionalProfileRepository : IGenericRepository<ProfessionalProfile>
{
    Task<ProfessionalProfile?> GetByDocumentAsync(string documentNumber);
    Task<ProfessionalProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<ProfessionalProfile?> GetBySubscriptionIdAsync(string subscriptionId, CancellationToken ct);
}
