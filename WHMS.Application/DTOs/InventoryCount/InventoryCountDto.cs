namespace WHMS.Application.DTOs.InventoryCount;

public class InventoryCountDto
{
    public string InventoryCountId { get; set; } = null!;
    public string WarehouseId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string? CreatedById { get; set; }
}