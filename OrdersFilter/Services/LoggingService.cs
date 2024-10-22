namespace OrdersFilter.Services;

public class LoggingService : ILoggingService
{
    private readonly string _logFilePath;
    private readonly object _lock = new object();

    public LoggingService(string logFilePath)
    {
        _logFilePath = logFilePath;
    }

    public Task LogAsync(string message)
    {
        return Task.Run(() =>
        {
            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
            lock (_lock)
            {
                File.AppendAllText(_logFilePath, logMessage);
            }
        });
    }
}