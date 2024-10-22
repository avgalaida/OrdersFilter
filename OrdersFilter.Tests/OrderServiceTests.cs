using System;
using OrdersFilter.Services;
using OrdersFilter.Models;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdersFilter.Tests;

public class OrderServiceTests
{
    [Fact]
    public async Task FilterOrdersAsync_ReturnsFilteredOrders()
    {
        var mockFileService = new Mock<IFileService>();
        var mockLogger = new Mock<ILoggingService>();

        var orders = new List<Order>
        {
            new Order
            {
                Number = 1001, Weight = 2.5, CityDistrict = "5",
                DeliveryDateTime = DateTime.Parse("2024-10-22 14:35:00")
            },
            new Order
            {
                Number = 1002, Weight = 1.0, CityDistrict = "3",
                DeliveryDateTime = DateTime.Parse("2024-10-22 14:40:00")
            },
            new Order
            {
                Number = 1003, Weight = 3.2, CityDistrict = "5",
                DeliveryDateTime = DateTime.Parse("2024-10-22 14:45:00")
            }
        };

        mockFileService.Setup(fs => fs.ReadAllLinesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<string>
            {
                "1001,2.5,5,2024-10-22 14:35:00",
                "1002,1.0,3,2024-10-22 14:40:00",
                "1003,3.2,5,2024-10-22 14:45:00"
            });

        var orderService = new OrderService(mockFileService.Object);
        var criteria = new FilterCriteria
        {
            CityDistrict = "5",
            FirstDeliveryDateTime = DateTime.Parse("2024-10-22 14:30:00")
        };

        var filteredOrders = await orderService.FilterOrdersAsync("dummyPath", criteria, mockLogger.Object);

        Assert.Equal(2, filteredOrders.Count);
        Assert.Contains(filteredOrders, o => o.Number == 1001);
        Assert.Contains(filteredOrders, o => o.Number == 1003);
    }
}