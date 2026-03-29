
using WHMS.Application.DTOs.Delivery;

namespace WHMS.Application.Features.Queries.Delivery.GetDelivery;

public class GetDeliveryQueryResponse
{
    public string WarehouseId { get; set; } = null!;
    public string WarehouseName { get; set; } = null!;
    public DateTime ExpectedArrivalDate { get; set; }
    public bool IsReceived { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public string? ReceivedById { get; set; }
}