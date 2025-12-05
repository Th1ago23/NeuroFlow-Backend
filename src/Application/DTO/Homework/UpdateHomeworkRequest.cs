namespace Application.DTO.Homework;

public sealed record UpdateHomeworkRequest(string Title,string? Description,DateTime ExpirationTime);
