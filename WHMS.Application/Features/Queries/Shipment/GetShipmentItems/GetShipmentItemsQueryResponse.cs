using WHMS.Application.DTOs.Shipment;

namespace WHMS.Application.Features.Queries.Shipment.GetShipmentItems;

public class GetShipmentItemsQueryResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPage { get; set; }
    public List<ShipmentItemDto>? ShipmentItems { get; set; }
}