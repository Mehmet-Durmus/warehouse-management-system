namespace WHMS.Application.Features.Command.Delivery.UpdateDelivery;

public class UpdateDeliveryCommandResponse
{
    public string WarehouseId { get; set; } = null!;
    public string WarehouseName { get; set; } = null!;
    public DateTime ExpectedArrivalDate { get; set; }
}