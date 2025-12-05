using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IHomeworkRepository:IGenericRepository<Homework>
{
    Task<Homework?> GetByIdAsync(Guid id);
    Task<IEnumerable<Homework>> GetByPatientAsync(Guid patientId);
    Task<IEnumerable<Homework>> GetByProfessionalAsync(Guid professionalUserId);
    void Update(Homework homework);
    void Remove(Homework homework);
}
