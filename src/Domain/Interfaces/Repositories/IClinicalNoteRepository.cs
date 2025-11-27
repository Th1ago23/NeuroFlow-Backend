using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IClinicalNoteRepository : IGenericRepository<ClinicalNote>
{
    Task<IEnumerable<ClinicalNote>> GetByPatientAsync(Guid patientId);
}
