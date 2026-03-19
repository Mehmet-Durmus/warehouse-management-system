using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCount;

public class GetInventoryCountQueryHandler : IRequestHandler<GetInventoryCountQueryRequest, GetInventoryCountQueryResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;

    public GetInventoryCountQueryHandler(IInventoryCountRepository inventoryCountRepository)
    {
        _inventoryCountRepository = inventoryCountRepository;
    }

    public async Task<GetInventoryCountQueryResponse> Handle(GetInventoryCountQueryRequest request, CancellationToken cancellationToken)
    {
        var inventoryCount = await _inventoryCountRepository.GetInventoryCount(Guid.Parse(request.InventoryCountId!));
        if (inventoryCount is null)
            throw new Exception("Inventory count not found.");

        return new()
        {
            WarehouseId = inventoryCount.WarehouseId.ToString(),
            CreatedAt = inventoryCount.CreatedAt,
            CreatedById = inventoryCount.CreatedById.ToString(),
            CreatedByName = inventoryCount.CreatedByName,
            CreatedByUserName = inventoryCount.CreatedByUserName,
            UpdatedAt = inventoryCount.UpdatedAt,
            UpdatedById = inventoryCount.UpdatedById.ToString(),
            UpdatedByName = inventoryCount.UpdatedByName,
            UpdatedByUserName = inventoryCount.UpdatedByUserName
        };
    }
}