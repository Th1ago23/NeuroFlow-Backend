using Application.DTO.Clinical;

namespace Application.Interfaces.Clinical;

public interface IClinicalNoteService
{
    Task<Guid> CreateAsync(CreateClinicalNoteRequest request);
    Task<IEnumerable<ClinicalNoteDto>> GetByPatientAsync(Guid patientId);
}
