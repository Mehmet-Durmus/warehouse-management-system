using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCountLine;

public class GetInventoryCountLineQueryHandler : IRequestHandler<GetInventoryCountLineQueryRequest, GetInventoryCountLineQueryResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;

    public GetInventoryCountLineQueryHandler(IInventoryCountRepository inventoryCountRepository)
    {
        _inventoryCountRepository = inventoryCountRepository;
    }

    public async Task<GetInventoryCountLineQueryResponse> Handle(GetInventoryCountLineQueryRequest request, CancellationToken cancellationToken)
    {
        var inventoryCountLine = await _inventoryCountRepository.GetInventoryCountLine(Guid.Parse(request.InventoryCountLineId!));
        if (inventoryCountLine is null)
            throw new Exception("Inventory count line not found.");
        
        return new()
        {
            InventoryCountId = inventoryCountLine.InventoryCountId.ToString(),
            SkuId = inventoryCountLine.SkuId.ToString(),
            Quantity = inventoryCountLine.Quantity,
            CreatedAt = inventoryCountLine.CreatedAt,
            CreatedById = inventoryCountLine.CreatedById.ToString()
        };
    }
}