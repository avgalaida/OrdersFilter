using OrdersFilter.Models;
using OrdersFilter.Services;
using OrdersFilter.Utils;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace OrdersFilter;

class Program
{
    static async Task<int> Main(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        string inputFilePath;
        string logFilePath;
        string resultFilePath;

        if (args.Length == 2)
        {
            string cityDistrict = args[0];
            string firstDeliveryDateTimeStr = args[1];

            if (!Validator.ValidateFilterCriteria(cityDistrict, firstDeliveryDateTimeStr, out FilterCriteria criteria,
                    out string validationError))
            {
                Console.WriteLine($"Ошибка валидации: {validationError}");
                return 1;
            }

            inputFilePath = config["DeliveryOrderPath"] ?? "input/orders.txt";
            logFilePath = config["DeliveryLogPath"] ?? "output/log.txt";
            resultFilePath = config["ResultFilePath"] ?? "output/result.txt";

            await ProcessOrdersAsync(criteria, inputFilePath, logFilePath, resultFilePath);
        }
        else
        {
            Console.WriteLine(
                "Неверное количество аргументов. Пытаемся загрузить параметры из конфигурации или переменных среды.");

            string cityDistrict = config["CityDistrict"];
            string firstDeliveryDateTimeStr = config["FirstDeliveryDateTime"];

            if (string.IsNullOrWhiteSpace(cityDistrict) || string.IsNullOrWhiteSpace(firstDeliveryDateTimeStr))
            {
                cityDistrict = Environment.GetEnvironmentVariable("CityDistrict") ?? cityDistrict;
                firstDeliveryDateTimeStr = Environment.GetEnvironmentVariable("FirstDeliveryDateTime") ??
                                           firstDeliveryDateTimeStr;
            }

            if (string.IsNullOrWhiteSpace(cityDistrict) || string.IsNullOrWhiteSpace(firstDeliveryDateTimeStr))
            {
                Console.WriteLine(
                    "Не удалось получить необходимые параметры для фильтрации (CityDistrict и FirstDeliveryDateTime).");
                Console.WriteLine("Проверьте аргументы командной строки, файл конфигурации или переменные среды.");
                return 1;
            }

            if (!Validator.ValidateFilterCriteria(cityDistrict, firstDeliveryDateTimeStr, out FilterCriteria criteria,
                    out string validationError))
            {
                Console.WriteLine($"Ошибка валидации: {validationError}");
                return 1;
            }

            inputFilePath = config["DeliveryOrderPath"] ??
                            Environment.GetEnvironmentVariable("DeliveryOrderPath") ?? "input/orders.txt";
            logFilePath = config["DeliveryLogPath"] ??
                          Environment.GetEnvironmentVariable("DeliveryLogPath") ?? "output/log.txt";
            resultFilePath = config["ResultFilePath"] ??
                             Environment.GetEnvironmentVariable("ResultFilePath") ?? "output/result.txt";

            await ProcessOrdersAsync(criteria, inputFilePath, logFilePath, resultFilePath);
        }

        Console.WriteLine("Фильтрация заказов завершена. Проверьте result.txt и log.txt для подробностей.");
        return 0;
    }

    private static async Task ProcessOrdersAsync(FilterCriteria criteria, string inputFilePath, string logFilePath,
        string resultFilePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(inputFilePath) ?? string.Empty);
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath) ?? string.Empty);
        Directory.CreateDirectory(Path.GetDirectoryName(resultFilePath) ?? string.Empty);

        ILoggingService logger = new LoggingService(logFilePath);
        IFileService fileService = new FileService();
        IOrderService orderService = new OrderService(fileService);

        await logger.LogAsync("Начало обработки заказов.");

        try
        {
            var filteredOrders = await orderService.FilterOrdersAsync(inputFilePath, criteria, logger);

            var batchSize = 1000;
            var batches = filteredOrders.Select((order, index) => new { order, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x =>
                    $"{x.order.Number},{x.order.Weight.ToString(CultureInfo.InvariantCulture)},{x.order.CityDistrict},{x.order.DeliveryDateTime:yyyy-MM-dd HH:mm:ss}"));

            foreach (var batch in batches)
            {
                await fileService.WriteLinesAsync(resultFilePath, batch);
            }

            await logger.LogAsync($"Результаты записаны в {resultFilePath}");
        }
        catch (Exception ex)
        {
            await logger.LogAsync($"Необработанная ошибка: {ex.Message}");
            throw;
        }

        await logger.LogAsync("Обработка завершена успешно.");
    }
}