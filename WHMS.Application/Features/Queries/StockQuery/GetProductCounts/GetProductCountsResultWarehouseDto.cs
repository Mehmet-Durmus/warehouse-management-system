namespace WHMS.Application.Features.Queries.StockQuery.GetProductCounts;

public class GetProductCountsResultWarehouseDto
{
    public string WarehouseId { get; set; } = null!;
    public string WarehouseName { get; set; } = null!;
    public int Quantity { get; set; }
    public List<GetProductCountsResultCategoryDto> Categories { get; set; } = null!;
}