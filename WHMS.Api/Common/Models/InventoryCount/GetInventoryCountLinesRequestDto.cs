namespace WHMS.Api.Common.Models.InventoryCount;

public class GetInventoryCountLinesRequestDto
{
    public string? SkuId { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public int? MinVariance { get; set; }
    public int? MaxVariance { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}