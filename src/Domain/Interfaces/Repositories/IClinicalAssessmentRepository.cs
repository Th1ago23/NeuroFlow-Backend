using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IClinicalAssessmentRepository : IGenericRepository<ClinicalAssessment>
{
    Task<IEnumerable<ClinicalAssessment>> GetByPatientAsync(Guid patientId);
}
