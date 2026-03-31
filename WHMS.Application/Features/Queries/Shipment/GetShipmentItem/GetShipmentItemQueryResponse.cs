namespace WHMS.Application.Features.Queries.Shipment.GetShipmentItem;

public class GetShipmentItemQueryResponse
{
    public string ShimpentId { get; set; } = null!;
    public GetShipmentItemResultSkuDto Sku { get; set; } = null!;
    public int Quantity { get; set; }
}