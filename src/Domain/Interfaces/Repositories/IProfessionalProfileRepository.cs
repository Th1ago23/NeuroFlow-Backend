using Domain.Entities.Profiles;

namespace Domain.Interfaces.Repositories;

public interface IProfessionalProfileRepository : IGenericRepository<ProfessionalProfile>
{
    Task<ProfessionalProfile?> GetByDocumentAsync(string documentNumber);
}
