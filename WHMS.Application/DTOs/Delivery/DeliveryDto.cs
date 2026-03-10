namespace WHMS.Application.DTOs.Delivery;

public class DeliveryDto
{
    public string DeliveryId { get; set; } = null!;
    public string WarehouseId { get; set; } = null!;
    public string StoreId { get; set; } = null!;
    public List<DeliveryItemDto> DeliveryItems { get; set; } = null!;
    public DateTime? ReceivedAt { get; set; }
    public string? ReceivedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedById { get; set; } = null!;
    public string CreatedByName { get; set; } = null!;
    public string CreatedByUserName { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
    public string? UpdatedById { get; set; }
    public string UpdatedByName { get; set; } = null!;
    public string UpdatedByUserName { get; set; } = null!;
}