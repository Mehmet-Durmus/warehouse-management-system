namespace WHMS.Application.Features.Queries.Delivery.GetDeliveries;

public class GetDeliveriesResultDeliveryDto
{
    public string DeliveryId { get; set; } = null!;
    public string WarehouseId { get; set; } = null!;
    public string WarehouseName { get; set; } = null!;
    public DateTime ExpectedArrivalDate { get; set; }
    public bool IsReceived { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public string? ReceivedById { get; set; }
}