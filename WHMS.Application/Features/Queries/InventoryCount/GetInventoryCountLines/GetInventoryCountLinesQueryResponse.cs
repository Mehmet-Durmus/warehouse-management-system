using WHMS.Application.DTOs.InventoryCount;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCountLines;

public class GetInventoryCountLinesQueryResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPage { get; set; }
    public List<InventoryCountLineDto>? InventoryCountLines { get; set; }
}