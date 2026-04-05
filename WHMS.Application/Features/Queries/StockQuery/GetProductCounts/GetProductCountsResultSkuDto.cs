namespace WHMS.Application.Features.Queries.StockQuery.GetProductCounts;

public class GetProductCountsResultSkuDto
{
    public string SkuId { get; set; } = null!;
    public string SkuName { get; set; } = null!;
    public int Quantity { get; set; }
}