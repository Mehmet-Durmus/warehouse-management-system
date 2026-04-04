using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCount;

public class GetInventoryCountQueryHandler : IRequestHandler<GetInventoryCountQueryRequest, GetInventoryCountQueryResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetInventoryCountQueryHandler(IInventoryCountRepository inventoryCountRepository, ICurrentUserService currentUserService)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetInventoryCountQueryResponse> Handle(GetInventoryCountQueryRequest request, CancellationToken cancellationToken)
    {
        var inventoryCount = await _inventoryCountRepository.GetInventoryCount(Guid.Parse(request.InventoryCountId!));
        bool notFound = inventoryCount switch
        {
            null => true,
            not null when _currentUserService.Roles!.Contains("WarehouseManager")
                && inventoryCount.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains("WarehouseStaff")
                && inventoryCount.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains("WarehouseStaff")
                && inventoryCount.WarehouseId == Guid.Parse(_currentUserService.WarehouseId!)
                && !inventoryCount.IsCompleted => true,
            _ => false
        };

        if (notFound)
            throw new Exception("Inventory count not found.");

        return new()
        {
            WarehouseId = inventoryCount!.WarehouseId.ToString(),
            IsCompleted = inventoryCount.IsCompleted,
            CreatedAt = inventoryCount.CreatedAt,
            CreatedById = inventoryCount.CreatedById.ToString()            
        };
    }
}