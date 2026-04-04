using MediatR;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCounts;

public class GetInventoryCountsQueryRequest : IRequest<GetInventoryCountsQueryResponse>
{
    public string? WarehouseId { get; set; }
    public bool? IsDone { get; set; }
    public List<string>? SkuIds { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? UpdatedAfter { get; set; }
    public DateTime? UpdatedBefore { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}