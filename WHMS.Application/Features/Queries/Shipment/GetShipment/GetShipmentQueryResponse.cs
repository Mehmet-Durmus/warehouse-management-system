using WHMS.Application.DTOs.Shipment;

namespace WHMS.Application.Features.Queries.Shipment.GetShipment;

public class GetShipmentQueryResponse
{
    public string WarehouseId { get; set; } = null!;
    public string WarehouseName { get; set; } = null!;
    public string StoreId { get; set; } = null!;
    public string StoreName { get; set; } = null!;
    public DateTime ExpectedSendingDate { get; set; }
    public DateTime? SendingDate { get; set; }
    public string? SentById { get; set; } = null!;
}