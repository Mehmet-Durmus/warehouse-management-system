using WHMS.Application.DTOs.Shipment;

namespace WHMS.Application.Features.Queries.Shipment.GetShipment;

public class GetShipmentQueryResponse
{
    public string WarehouseId { get; set; } = null!;
    public string StoreId { get; set; } = null!;
    public DateTime ExpectedSendingDate { get; set; }
    public DateTime? SendingDate { get; set; }
    public string? SentById { get; set; } = null!;
    public List<ShipmentItemDto>? ShipmentItems { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedById { get; set; } = null!;
    public string? CreatedByName { get; set; } = null!;
    public string? CreatedByUserName { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
    public string? UpdatedById { get; set; }
    public string? UpdatedByName { get; set; } = null!;
    public string? UpdatedByUserName { get; set; } = null!;
}