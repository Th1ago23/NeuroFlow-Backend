namespace Application.Common.Responses;
public class ApiResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? ErrorCode { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    private ApiResponse(bool success, string message, string? errorCode)
    {
        Success = success;
        Message = message;
        ErrorCode = errorCode;
    }

    public static ApiResponse Ok(string message = "Success")
        => new(true, message, null);

    public static ApiResponse Fail(string message, string? errorCode = null)
        => new(false, message, errorCode);
}
