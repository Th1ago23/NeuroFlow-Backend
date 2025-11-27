namespace Application.DTO.Clinical;

public sealed record ClinicalNoteDto(
    Guid Id,
    string Note,
    DateTime CreatedAt
);
