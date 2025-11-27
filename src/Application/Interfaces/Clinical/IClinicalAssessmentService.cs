using Application.DTO.Clinical;

namespace Application.Interfaces.Clinical;

public interface IClinicalAssessmentService
{
    Task<Guid> CreateAsync(CreateClinicalAssessmentRequest request);
    Task<IEnumerable<ClinicalAssessmentDto>> GetByPatientAsync(Guid patientId);
}
