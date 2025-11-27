namespace Application.DTO.Clinical;
public sealed record CreateClinicalAssessmentRequest(
    Guid ProfessionalUserId,
    Guid PatientId,
    string Summary,
    string? Diagnosis,
    string? Recommendations
);
