using MediatR;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCountLine;

public class GetInventoryCountLineQueryRequest : IRequest<GetInventoryCountLineQueryResponse>
{
    public string? InventoryCountLineId { get; set; }
}