using WHMS.Application.Common.DTOs;
using WHMS.Application.DTOs.InventoryCount;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCountLines;

public class GetInventoryCountLinesQueryResponse
{
    public PaginationDto Pagination { get; set; } = null!;
    public List<GetInventoryCountLinesResultInventoryCountLineDto> InventoryCountLines { get; set; } = null!;
}