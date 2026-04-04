using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.InventoryCount.UpdateInventoryCountLine;

public class UpdateInventoryCountLineCommandHandler : IRequestHandler<UpdateInventoryCountLineCommandRequest, UpdateInventoryCountLineCommandResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateInventoryCountLineCommandHandler(IInventoryCountRepository inventoryCountRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<UpdateInventoryCountLineCommandResponse> Handle(UpdateInventoryCountLineCommandRequest request, CancellationToken cancellationToken)
    {
        var newInventoryCount = await _inventoryCountRepository.GetInventoryCount(Guid.Parse(request.InventoryCountId!));
        bool notFoundNewInventoryCount = newInventoryCount switch
        {
            null => true,
            not null when (_currentUserService.Roles!.Contains("WarehouseManager")
                || _currentUserService.Roles!.Contains("WarehouseStaff"))
                && newInventoryCount.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            _ => false
        };
        if (notFoundNewInventoryCount)
            throw new Exception("Inventory count not found.");

        if (newInventoryCount!.IsCompleted)
            throw new Exception($"{request.InventoryCountId}: Inventory count has already been completed.");
        
        var inventoryCountLine = await _inventoryCountRepository.GetInventoryCountLine(Guid.Parse(request.InventoryCountLineId!));
        bool notFoundInventoryCountLine = inventoryCountLine switch
        {
            null => true,
            not null when _currentUserService.Roles!.Contains("WarehouseStaff")
                && inventoryCountLine.CreatedById != _currentUserService.UserId => true,
            _ => false
        };
        
        if (notFoundInventoryCountLine)
            throw new Exception("Inventory count line not found.");

        if (inventoryCountLine!.InventoryCountId != Guid.Parse(request.InventoryCountId!))
        {
            var oldInventoryCount = await _inventoryCountRepository.GetInventoryCount(inventoryCountLine.InventoryCountId);
            bool notFoundOldInventoryCount = oldInventoryCount switch
            {
                null => true,
                not null when (_currentUserService.Roles!.Contains("WarehouseManager")
                    || _currentUserService.Roles!.Contains("WarehouseStaff"))
                    && oldInventoryCount.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
                _ => false
            };
            if (notFoundOldInventoryCount)
                throw new Exception("Inventory count not found.");

            if (oldInventoryCount!.IsCompleted)
                throw new Exception($"{inventoryCountLine.InventoryCountId}: Inventory count has already been completed.");
        }

        inventoryCountLine.InventoryCountId = Guid.Parse(request.InventoryCountId!);
        inventoryCountLine.SkuId = Guid.Parse(request.SkuId!);
        inventoryCountLine.Quantity = request.Quantity;
        
        await _unitOfWork.CommitAsync();
        return new()
        {   InventoryCountId = inventoryCountLine.InventoryCountId.ToString(),
            SkuId = inventoryCountLine.SkuId.ToString(),
            Quantity = inventoryCountLine.Quantity,
            Variance = inventoryCountLine.Variance
        };
    }
}