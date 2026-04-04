namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCount;

public class GetInventoryCountQueryResponse
{
    public string WarehouseId { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedById { get; set; }
}