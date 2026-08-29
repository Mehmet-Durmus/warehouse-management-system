using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.InventoryCount;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.InventoryCount.CreateInventoryCountLine;

public class CreateInventoryCountLineCommandHandler : IRequestHandler<CreateInventoryCountLineCommandRequest, CreateInventoryCountLineCommandResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStockStateRepository _stockStateRepository;

    public CreateInventoryCountLineCommandHandler(IInventoryCountRepository inventoryCountRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IStockStateRepository stockStateRepository)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _stockStateRepository = stockStateRepository;
    }

    public async Task<CreateInventoryCountLineCommandResponse> Handle(CreateInventoryCountLineCommandRequest request, CancellationToken cancellationToken)
    {
        Guid warehouseId = Guid.Parse(_currentUserService.WarehouseId!);
        var inventoryCount = InventoryCountRules.EnsureAccessibleForWarehouse(await _inventoryCountRepository.GetInventoryCount(Guid.Parse(request.InventoryCountId!)), warehouseId);
        InventoryCountRules.EnsureNotCompletedForLineCreation(inventoryCount.IsCompleted);

        int currentStock = await _stockStateRepository.GetStockQuantity(warehouseId, Guid.Parse(request.SkuId!));

        InventoryCountLine inventoryCountLine = new()
        {
            InventoryCountId = Guid.Parse(request.InventoryCountId!),
            SkuId = Guid.Parse(request.SkuId!),
            Quantity = request.Quantity,
            Variance = InventoryCountRules.CalculateVariance(request.Quantity, currentStock)
        };
        await _inventoryCountRepository.CreateInventoryCounLine(inventoryCountLine);
        await _unitOfWork.CommitAsync();
        return new()
        {
            InventoryCountId = inventoryCount.Id.ToString(),
            InventoryCountLineId = inventoryCountLine.Id.ToString(),
            SkuId = inventoryCountLine.SkuId.ToString(),
            Quantity = inventoryCountLine.Quantity,
            Variance = inventoryCountLine.Variance
        };
    }
}