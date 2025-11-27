namespace Application.DTO.Homework;

public sealed record CreateHomeworkRequest(
    Guid PatientId,
    string Title,
    string? Description,
    DateTime ExpirationTime
);
