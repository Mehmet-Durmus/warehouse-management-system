namespace WHMS.Application.Features.Queries.Shipment.GetShipmentItem;

public class GetShipmentItemQueryResponse
{
    public string? ShipmentId { get; set; }
    public string? SkuId { get; set; }
    public string? SKUName { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public string? CreatedByName { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }
    public string? UpdatedByName { get; set; }
    public string? UpdatedByUserName { get; set; }
}