using System.Globalization;

namespace OrdersFilter.Models;

public class Order
{
    public string Number { get; set; }
    public double Weight { get; set; }
    public string CityDistrict { get; set; }
    public DateTime DeliveryDateTime { get; set; }

    public static bool TryParse(string line, out Order order, out string error)
    {
        order = null;
        error = null;

        var parts = line.Split(',');

        if (parts.Length != 4)
        {
            error = $"Некорректное количество полей: {line}";
            return false;
        }

        var number = parts[0].Trim();
        if (string.IsNullOrEmpty(number))
        {
            error = $"Отсутствует номер заказа: {line}";
            return false;
        }

        if (!double.TryParse(parts[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double weight))
        {
            error = $"Некорректный вес: {line}";
            return false;
        }

        var cityDistrict = parts[2].Trim();
        if (string.IsNullOrEmpty(cityDistrict))
        {
            error = $"Отсутствует район доставки: {line}";
            return false;
        }

        if (!DateTime.TryParseExact(parts[3].Trim(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime deliveryDateTime))
        {
            error = $"Некорректное время доставки: {line}";
            return false;
        }

        order = new Order
        {
            Number = number,
            Weight = weight,
            CityDistrict = cityDistrict,
            DeliveryDateTime = deliveryDateTime
        };

        return true;
    }
}