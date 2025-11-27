namespace Application.DTO.Patients;

public sealed record PatientInviteDto(
    Guid Token,
    Guid ProfessionalUserId,
    DateTime ExpiresAt,
    bool IsUsed
);
