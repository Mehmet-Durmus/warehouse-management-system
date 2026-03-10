using WHMS.Application.DTOs.Delivery;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveries;

public class GetDeliveriesQueryResponse
{
    public List<DeliveryDto> Deliveries { get; set; } = null!;
}