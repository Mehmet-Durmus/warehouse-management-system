using MediatR;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCountLines;

public class GetInventoryCountLinesQueryRequest : IRequest<GetInventoryCountLinesQueryResponse>
{
    public string? InventoryCountId { get; set; }
    public string? SkuId { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public int? MinVariance { get; set; }
    public int? MaxVariance { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}