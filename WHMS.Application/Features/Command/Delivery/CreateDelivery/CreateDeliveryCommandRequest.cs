using MediatR;

namespace WHMS.Application.Features.Command.Delivery.CreateDelivery;

public class CreateDeliveryCommandRequest : IRequest<CreateDeliveryCommandResponse>
{
    public string? WarehouseId { get; set; }
    public DateTime ExpectedArrivalDate { get; set; }
}