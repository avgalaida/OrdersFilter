using OrdersFilter.Models;

namespace OrdersFilter.Services;

public class OrderService : IOrderService
{
    private readonly IFileService _fileService;

    public OrderService(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task FilterOrdersAsync(string inputFilePath, string outputFilePath, FilterCriteria criteria,
        ILoggingService logger)
    {
        DateTime endDateTime = criteria.GetEndDateTime();
        int processedLines = 0;
        int writtenOrders = 0;
        int batchSize = 1000;
        var batch = new List<string>(batchSize);

        try
        {
            await logger.LogAsync("Начало фильтрации заказов.");

            await foreach (var line in _fileService.ReadLinesAsync(inputFilePath))
            {
                processedLines++;

                if (Order.TryParse(line, out Order? order, out string? error))
                {
                    if (order != null &&
                        order.CityDistrict == criteria.CityDistrict &&
                        order.DeliveryDateTime >= criteria.FirstDeliveryDateTime &&
                        order.DeliveryDateTime <= endDateTime)
                    {
                        batch.Add(line);
                        writtenOrders++;
                    }
                }
                else
                {
                    await logger.LogAsync($"Ошибка на строке {processedLines}: {error}");
                }

                if (batch.Count >= batchSize)
                {
                    await _fileService.WriteLinesAsync(outputFilePath, batch);
                    batch.Clear();
                    await logger.LogAsync($"{processedLines} строк обработано, {writtenOrders} заказов записано.");
                }
            }

            if (batch.Count > 0)
            {
                await _fileService.WriteLinesAsync(outputFilePath, batch);
                await logger.LogAsync($"{processedLines} строк обработано, {writtenOrders} заказов записано.");
            }

            await logger.LogAsync(
                $"Фильтрация завершена. Обработано {processedLines} строк, записано {writtenOrders} заказов.");
        }
        catch (Exception ex)
        {
            await logger.LogAsync($"Ошибка при обработке файла: {ex.Message}");
            throw;
        }
    }
}