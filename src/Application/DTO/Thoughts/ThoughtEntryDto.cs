namespace Application.DTO.Thoughts;

public sealed record ThoughtEntryDto(
    Guid Id,
    int CategoryId,
    string CategoryName,
    string IconKey,
    string? Notes,
    bool IsVisibleToProfessional,
    DateTime CreatedAt
);
