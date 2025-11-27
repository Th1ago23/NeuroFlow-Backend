namespace Application.Common.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public T? Data { get; init; }
        public string? ErrorCode { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        private ApiResponse(bool success, string message, T? data, string? errorCode)
        {
            Success = success;
            Message = message;
            Data = data;
            ErrorCode = errorCode;
        }

        public static ApiResponse<T> Ok(T data, string message = "Success")
            => new(true, message, data, null);

        public static ApiResponse<T> Fail(string message, string? errorCode = null)
            => new(false, message, default, errorCode);
    }
}
