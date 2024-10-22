using OrdersFilter.Models;

namespace OrdersFilter.Services;

public interface IOrderService
{
    Task FilterOrdersAsync(string inputFilePath, string outputFilePath, FilterCriteria criteria,
        ILoggingService logger);
}