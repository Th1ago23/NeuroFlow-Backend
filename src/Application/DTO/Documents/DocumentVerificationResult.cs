namespace Application.DTO.Documents;

public record DocumentVerificationResult(bool IsValid, string? FullName, string? Speciality, string? Status);
