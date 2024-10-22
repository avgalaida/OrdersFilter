namespace OrdersFilter.Services;

public interface ILoggingService
{
    Task LogAsync(string message);
}