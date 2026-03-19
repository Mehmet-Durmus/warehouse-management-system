using MediatR;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCount;

public class GetInventoryCountQueryRequest : IRequest<GetInventoryCountQueryResponse>
{
    public string? InventoryCountId { get; set; }
}