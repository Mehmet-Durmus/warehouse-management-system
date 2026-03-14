using WHMS.Application.DTOs.Shipment;

namespace WHMS.Application.Features.Queries.Shipment.GetShipments;

public class GetShipmentsQueryResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPage { get; set; }
    public List<ShipmentDto>? Shipments { get; set; }
}