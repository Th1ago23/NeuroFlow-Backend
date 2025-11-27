namespace Application.DTO.Clinical;

public sealed record ClinicalAssessmentDto(
    Guid Id,
    Guid ProfessionalUserId,
    string Summary,
    string? Diagnosis,
    string? Recommendations,
    DateTime Date,
    DateTime CreatedAt
);
