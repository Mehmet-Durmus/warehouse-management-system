using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Queries.StockQuery.GetProductCounts;

public class GetProductCountsQueryResponse
{
    public int TotalQuantity { get; set; }
    public List<GetProductCountsResultWarehouseDto> Warehouses { get; set; } = null!;
}