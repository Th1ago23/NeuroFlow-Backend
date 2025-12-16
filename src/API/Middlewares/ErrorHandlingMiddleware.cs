using Application.Common.Responses;
using System.Net;
using System.Text.Json;

namespace API.Middlewares;

public class ErrorHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ErrorHandlingMiddleware(
        ILogger<ErrorHandlingMiddleware> logger,
        IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var requestId = context.TraceIdentifier;

        _logger.LogError(ex,
            "Unhandled exception. RequestId: {RequestId}, Path: {Path}",
            requestId,
            context.Request.Path);

        var code = HttpStatusCode.InternalServerError;
        var message = "Ocorreu um erro inesperado.";

        switch (ex)
        {
            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                message = "Acesso não autorizado.";
                break;

            case KeyNotFoundException:
                code = HttpStatusCode.NotFound;
                message = "Recurso não encontrado.";
                break;

            case ArgumentException or ArgumentNullException:
                code = HttpStatusCode.BadRequest;
                message = ex.Message;
                break;
        }

        context.Response.StatusCode = (int)code;
        context.Response.ContentType = "application/json";

        var response = ApiResponse.Fail(message, ex.GetType().Name);

        var payload = new Dictionary<string, object?>
        {
            ["success"] = response.Success,
            ["message"] = response.Message,
            ["errorCode"] = response.ErrorCode,
            ["timestamp"] = response.Timestamp,
            ["requestId"] = requestId
        };

        if (_env.IsDevelopment())
        {
            payload["stackTrace"] = ex.StackTrace;
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
