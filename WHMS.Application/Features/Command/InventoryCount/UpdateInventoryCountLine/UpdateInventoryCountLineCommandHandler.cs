using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Domain.BusinessRules.InventoryCount;

namespace WHMS.Application.Features.Command.InventoryCount.UpdateInventoryCountLine;

public class UpdateInventoryCountLineCommandHandler : IRequestHandler<UpdateInventoryCountLineCommandRequest, UpdateInventoryCountLineCommandResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStockStateRepository _stockStateRepository;

    public UpdateInventoryCountLineCommandHandler(IInventoryCountRepository inventoryCountRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IStockStateRepository stockStateRepository)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _stockStateRepository = stockStateRepository;
    }

    public async Task<UpdateInventoryCountLineCommandResponse> Handle(UpdateInventoryCountLineCommandRequest request, CancellationToken cancellationToken)
    {
        var newInventoryCount = InventoryCountRules.EnsureAccessibleForWarehouse(
            await _inventoryCountRepository.GetInventoryCount(Guid.Parse(request.InventoryCountId!)),
            Guid.Parse(_currentUserService.WarehouseId!));
        InventoryCountRules.EnsureNotCompletedForLineUpdate(newInventoryCount.IsCompleted, Guid.Parse(request.InventoryCountId!));

        bool restrictedToOwnLines = _currentUserService.Roles!.Contains(ApplicationRole.WarehouseStaff);
        var inventoryCountLine = InventoryCountRules.EnsureLineAccessible(
            await _inventoryCountRepository.GetInventoryCountLine(Guid.Parse(request.InventoryCountLineId!)),
            restrictedToOwnLines, _currentUserService.UserId);

        if (inventoryCountLine.InventoryCountId != Guid.Parse(request.InventoryCountId!))
        {
            var oldInventoryCount = InventoryCountRules.EnsureAccessibleForWarehouse(
                await _inventoryCountRepository.GetInventoryCount(inventoryCountLine.InventoryCountId),
                Guid.Parse(_currentUserService.WarehouseId!));
            InventoryCountRules.EnsureNotCompletedForLineUpdate(oldInventoryCount.IsCompleted, inventoryCountLine.InventoryCountId);
        }

        inventoryCountLine.InventoryCountId = Guid.Parse(request.InventoryCountId!);
        inventoryCountLine.SkuId = Guid.Parse(request.SkuId!);
        inventoryCountLine.Quantity = request.Quantity;

        int currentStock = await _stockStateRepository.GetStockQuantity(newInventoryCount.WarehouseId, inventoryCountLine.SkuId);
        inventoryCountLine.Variance = InventoryCountRules.CalculateVariance(inventoryCountLine.Quantity, currentStock);

        await _unitOfWork.CommitAsync();
        return new()
        {   InventoryCountId = inventoryCountLine.InventoryCountId.ToString(),
            SkuId = inventoryCountLine.SkuId.ToString(),
            Quantity = inventoryCountLine.Quantity,
            Variance = inventoryCountLine.Variance
        };
    }
}