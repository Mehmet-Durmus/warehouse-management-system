namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCount;

public class GetInventoryCountQueryResponse
{
    public string WarehouseId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string? CreatedById { get; set; }
    public string? CreatedByName { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? UpdatedById { get; set; }
    public string? UpdatedByName { get; set; }
    public string? UpdatedByUserName { get; set; }
}