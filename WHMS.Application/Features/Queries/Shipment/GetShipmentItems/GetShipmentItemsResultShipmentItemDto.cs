namespace WHMS.Application.Features.Queries.Shipment.GetShipmentItems;

public class GetShipmentItemsResultShipmentItemDto
{
    public string ShipmentItemId { get; set; } = null!;
    public GetShipmentItemsResultSkuDto Sku { get; set; } = null!;
    public int Quantity { get; set; }
}