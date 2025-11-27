using Application.DTO.Clinical;
using Domain.Entities;

namespace Application.Mappings.Clinical;

public static class ClinicalNoteMappings
{
    // Domain -> DTO
    public static ClinicalNoteDto ToDto(this ClinicalNote note)
        => new(
            Id: note.Id,
            Note: note.Note,
            CreatedAt: note.CreatedAt
        );

    // DTO -> Domain
    public static ClinicalNote ToEntity(this CreateClinicalNoteRequest request)
        => new ClinicalNote(
            patientId: request.PatientId,
            note: request.Note
        );
}
