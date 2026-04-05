namespace WHMS.Application.Features.Queries.StockQuery.GetProductCounts;

public class GetProductCountsResultCategoryDto
{
    public string CategoryId { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public int Quantity { get; set; }
    public List<GetProductCountsResultSkuDto> Skus { get; set; } = null!;
}