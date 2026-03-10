namespace WHMS.Application.Filters;

public class DeliveryFilter
{
    public string? WarehouseId { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public bool? IsReceived { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}