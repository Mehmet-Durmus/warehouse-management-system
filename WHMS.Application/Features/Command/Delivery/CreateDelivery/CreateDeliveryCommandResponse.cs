namespace WHMS.Application.Features.Command.Delivery.CreateDelivery;

public class CreateDeliveryCommandResponse
{
    public string DeliveryId { get; set; } = null!;
    public string WarehouseId { get; set; } = null!;
    public string WarehouseName { get; set; } = null!;
    public DateTime ExpectedArrivalDate { get; set; }
}