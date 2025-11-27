namespace Application.DTO.Thoughts;
public sealed record CreateThoughtEntryRequest(
    Guid PatientId,
    int CategoryId,
    string? Notes,
    bool IsVisibleToProfessional
);
