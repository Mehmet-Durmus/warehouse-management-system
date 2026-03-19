namespace WHMS.Application.DTOs.InventoryCount;

public class InventoryCountLineDto
{
    public string? InventoryCountLineId { get; set; }
    public string? InventoryCountId { get; set; }
    public string? SkuId { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedById { get; set; }
}