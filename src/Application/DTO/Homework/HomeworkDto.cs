namespace Application.DTO.Homework;

public sealed record HomeworkDto(
    Guid Id,
    string Title,
    string? Description,
    DateTime ExpirationTime,
    bool IsDone
);
