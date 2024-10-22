using System;
using OrdersFilter.Utils;
using OrdersFilter.Models;
using Xunit;

namespace OrdersFilter.Tests;

public class ValidatorTests
{
    [Theory]
    [InlineData("5", "2024-10-22 14:30:00", true)]
    [InlineData("", "2024-10-22 14:30:00", false)]
    [InlineData("5", "invalid-date", false)]
    [InlineData(null, "2024-10-22 14:30:00", false)]
    [InlineData("5", null, false)]
    public void ValidateFilterCriteria_Test(string cityDistrict, string firstDeliveryDateTimeStr, bool expectedIsValid)
    {
        var isValid = Validator.ValidateFilterCriteria(cityDistrict, firstDeliveryDateTimeStr,
            out FilterCriteria criteria, out string error);

        Assert.Equal(expectedIsValid, isValid);
        if (expectedIsValid)
        {
            Assert.NotNull(criteria);
            Assert.Equal(cityDistrict, criteria.CityDistrict);
            Assert.Equal(DateTime.Parse(firstDeliveryDateTimeStr), criteria.FirstDeliveryDateTime);
            Assert.Null(error);
        }
        else
        {
            Assert.Null(criteria);
            Assert.False(string.IsNullOrEmpty(error));
        }
    }
}