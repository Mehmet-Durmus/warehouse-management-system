using MediatR;

namespace WHMS.Application.Features.Command.Delivery.UpdateDelivery;

public class UpdateDeliveryCommandRequest : IRequest<UpdateDeliveryCommandResponse>
{
    public string? DeliveryId { get; set; }
    public string? WarehouseId { get; set; }
    public DateTime ExpectedArrivalDate { get; set; }
}