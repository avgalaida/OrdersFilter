using OrdersFilter.Models;

namespace OrdersFilter.Utils;

public static class Validator
{
    public static bool ValidateFilterCriteria(string cityDistrict, string firstDeliveryDateTimeStr,
        out FilterCriteria criteria, out string error)
    {
        criteria = null;
        error = null;

        if (string.IsNullOrWhiteSpace(cityDistrict))
        {
            error = "Район доставки не может быть пустым.";
            return false;
        }

        if (!DateTime.TryParseExact(firstDeliveryDateTimeStr, "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                out DateTime firstDeliveryDateTime))
        {
            error = "Некорректный формат времени первой доставки. Ожидается формат: yyyy-MM-dd HH:mm:ss";
            return false;
        }

        criteria = new FilterCriteria
        {
            CityDistrict = cityDistrict,
            FirstDeliveryDateTime = firstDeliveryDateTime
        };

        return true;
    }
}