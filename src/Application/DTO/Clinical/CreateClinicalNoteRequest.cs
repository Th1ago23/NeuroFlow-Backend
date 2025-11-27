namespace Application.DTO.Clinical;
public sealed record CreateClinicalNoteRequest(
    Guid PatientId,
    string Note
);
