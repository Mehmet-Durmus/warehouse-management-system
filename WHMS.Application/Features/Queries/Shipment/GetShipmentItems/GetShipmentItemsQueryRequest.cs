using MediatR;

namespace WHMS.Application.Features.Queries.Shipment.GetShipmentItems;

public class GetShipmentItemsQueryRequest : IRequest<GetShipmentItemsQueryResponse>
{
    public string? ShipmentId { get; set; }
    public string? SkuId { get; set; }
    public int? MaxQuantity { get; set; }
    public int? MinQuantity { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}