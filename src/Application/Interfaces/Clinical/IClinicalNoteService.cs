using Application.DTO.Clinical;

namespace Application.Interfaces.Clinical;

public interface IClinicalNoteService
{
    Task<Guid> CreateAsync(CreateClinicalNoteRequest request,Guid requesterId,string requesterRole);

    Task<IEnumerable<ClinicalNoteDto>> GetByPatientAsync(Guid patientId,Guid requesterId,string requesterRole);

    Task UpdateAsync(Guid noteId,string newText,Guid requesterId,string requesterRole);

    Task DeleteAsync(Guid noteId,Guid requesterId,string requesterRole);
}
