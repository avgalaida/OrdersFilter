namespace OrdersFilter.Models;

public class FilterCriteria
{
    public string CityDistrict { get; set; }
    public DateTime FirstDeliveryDateTime { get; set; }

    public DateTime GetEndDateTime()
    {
        return FirstDeliveryDateTime.AddMinutes(30);
    }
}