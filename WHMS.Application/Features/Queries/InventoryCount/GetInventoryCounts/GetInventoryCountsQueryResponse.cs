using WHMS.Application.DTOs.InventoryCount;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCounts;

public class GetInventoryCountsQueryResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPage { get; set; }
    public List<InventoryCountDto>? InventoryCounts { get; set; } 
}