using MediatR;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCounts;

public class GetInventoryCountsQueryRequest : IRequest<GetInventoryCountsQueryResponse>
{
    public string? WarehouseId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}