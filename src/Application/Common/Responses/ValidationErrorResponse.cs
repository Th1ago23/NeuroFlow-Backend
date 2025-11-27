namespace Application.Common.Responses;

public sealed class ValidationErrorResponse
{
    public bool Success { get; init; } = false;
    public string Message { get; init; } = "Erro de validação.";
    public Dictionary<string, string[]> Errors { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public ValidationErrorResponse(Dictionary<string, string[]> errors)
    {
        Errors = errors;
    }
}
