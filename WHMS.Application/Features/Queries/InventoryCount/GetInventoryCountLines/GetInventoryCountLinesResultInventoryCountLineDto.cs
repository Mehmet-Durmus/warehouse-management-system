namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCountLines;

public class GetInventoryCountLinesResultInventoryCountLineDto
{
    public string InventoryCountLineId { get; set; } = null!;
    public string SkuId { get; set; } = null!;
    public string SkuName { get; set; } = null!;
    public int Quantity { get; set; }
    public int Variance { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedById { get; set; } = null!;
    public string CreatedByName { get; set; } = null!;
}