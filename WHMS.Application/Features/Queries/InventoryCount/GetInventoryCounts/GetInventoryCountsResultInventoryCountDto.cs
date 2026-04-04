namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCounts;

public class GetInventoryCountsResultInventoryCountDto
{
    public string InventoryCountId { get; set; } = null!;
    public string WarehouseId { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedById { get; set; }
}