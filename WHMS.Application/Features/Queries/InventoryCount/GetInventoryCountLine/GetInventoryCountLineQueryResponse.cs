namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCountLine;

public class GetInventoryCountLineQueryResponse
{
    public string InventoryCountId { get; set; } = null!;
    public string SkuId { get; set; } = null!;
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedById { get; set; }
}