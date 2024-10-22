using OrdersFilter.Models;

namespace OrdersFilter.Services;

public interface IOrderService
{
    Task<IEnumerable<Order>> FilterOrdersAsync(string inputFilePath, FilterCriteria criteria, ILoggingService logger);
}