namespace Application.DTO.Clinical;

public sealed record UpdateClinicalAssessmentRequest(
    string Summary,
    string? Diagnosis,
    string? Recommendations
);
