namespace WHMS.Application.Common.Filtering.Filters;

public class DeliveryFilter : QueryFilter
{
    public string? WarehouseId { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public string? NeighborhoodId { get; set; }
    public bool? IsReceived { get; set; }
    public DateTime? ReceivedAfter { get; set; }
    public DateTime? ReceivedBefore { get; set; }
    public DateTime? ExpectedArrivalAfter { get; set; }
    public DateTime? ExpectedArrivalBefore { get; set; }
}