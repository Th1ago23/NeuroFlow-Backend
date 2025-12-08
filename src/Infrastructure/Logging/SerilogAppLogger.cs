using Application.Interfaces.Logging;
using Serilog;

namespace Infrastructure.Logging;

public class SerilogAppLogger<T> : IAppLogger<T>
{
    public void LogInformation(string message, params object[] args)
    {
        Log.Information(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        Log.Warning(message, args);
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        Log.Error(exception, message, args);
    }
}
