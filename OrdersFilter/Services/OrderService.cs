using OrdersFilter.Models;

namespace OrdersFilter.Services;

public class OrderService : IOrderService
{
    private readonly IFileService _fileService;

    public OrderService(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<IEnumerable<Order>> FilterOrdersAsync(string inputFilePath, FilterCriteria criteria,
        ILoggingService logger)
    {
        var filteredOrders = new List<Order>();
        DateTime endDateTime = criteria.GetEndDateTime();

        try
        {
            using (var stream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(stream))
            {
                string line;
                int lineNumber = 0;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;
                    if (Order.TryParse(line, out Order order, out string error))
                    {
                        if (order.CityDistrict == criteria.CityDistrict &&
                            order.DeliveryDateTime >= criteria.FirstDeliveryDateTime &&
                            order.DeliveryDateTime <= endDateTime)
                        {
                            filteredOrders.Add(order);
                        }
                    }
                    else
                    {
                        await logger.LogAsync($"Ошибка на строке {lineNumber}: {error}");
                    }

                    if (lineNumber % 100 == 0)
                    {
                        await logger.LogAsync($"{lineNumber} строк обработано.");
                    }
                }
            }

            await logger.LogAsync("Фильтрация заказов завершена.");
        }
        catch (Exception ex)
        {
            await logger.LogAsync($"Ошибка при обработке файла: {ex.Message}");
            throw;
        }

        return filteredOrders;
    }
}