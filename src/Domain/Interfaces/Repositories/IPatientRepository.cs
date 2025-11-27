using Domain.Entities;

namespace Domain.Interfaces.Repositories;
public interface IPatientRepository : IGenericRepository<Patient>
{
    Task<IEnumerable<Patient>> GetByProfessionalIdAsync(Guid professionalUserId);
}
