using WHMS.Application.Common.DTOs;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCounts;

public class GetInventoryCountsQueryResponse
{
    public PaginationDto Pagination { get; set; } = null!;
    public List<GetInventoryCountsResultInventoryCountDto> InventoryCounts { get; set; } = null!;
}