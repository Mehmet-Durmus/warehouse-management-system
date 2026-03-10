namespace WHMS.Application.Features.Command.Delivery.CreateDelivery;

public class CreateDeliveryCommandResponse
{
    public string Id { get; set; } = null!;
    public string WarehouseId { get; set; } = null!;
    public string CreatedByUserName { get; set; } = null!;
}