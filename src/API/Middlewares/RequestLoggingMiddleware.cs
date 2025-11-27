using System.Diagnostics;

namespace API.Middlewares;
public class RequestLoggingMiddleware : IMiddleware
{
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();

        await next(context);

        stopwatch.Stop();

        _logger.LogInformation(
            "Request {Method} {Path} completed in {Elapsed} ms",
            context.Request.Method,
            context.Request.Path,
            stopwatch.ElapsedMilliseconds
        );
    }
}
