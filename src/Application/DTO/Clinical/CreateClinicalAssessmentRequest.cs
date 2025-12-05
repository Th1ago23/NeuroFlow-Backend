namespace Application.DTO.Clinical;
public sealed record CreateClinicalAssessmentRequest(
    Guid ProfessionalUserId,
    Guid PatientId,
    DateTime Date,
    string Summary,
    string? Diagnosis,
    string? Recommendations
);
