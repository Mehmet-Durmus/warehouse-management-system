namespace WHMS.Application.Features.Queries.Shipment.GetShipmentItems;

public class GetShipmentItemsResultSkuDto
{
    public string Id { get; set; } = null!;
    public string SKUName { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public decimal UnitPrice { get; set; }
}