using System.ComponentModel;
using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.InventoryCount;

namespace WHMS.Application.Features.Command.InventoryCount.CompleteInventoryCount;

public class CompleteInventoryCountCommandHandler : IRequestHandler<CompleteInventoryCountCommandRequest>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly IStockStateRepository _stockStateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CompleteInventoryCountCommandHandler(IInventoryCountRepository inventoryCountRepository, IStockStateRepository stockStateRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _stockStateRepository = stockStateRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(CompleteInventoryCountCommandRequest request, CancellationToken cancellationToken)
    {
        Guid warehouseId = Guid.Parse(_currentUserService.WarehouseId!);
        var inventoryCount = InventoryCountRules.EnsureAccessibleForWarehouse(await _inventoryCountRepository.GetInventoryCount(Guid.Parse(request.InventoryCountId!)), warehouseId);

        inventoryCount.IsCompleted = true;

        if (inventoryCount.InventoryCountLines is not null)
            foreach (var countLine in inventoryCount.InventoryCountLines)
                await _stockStateRepository.SetQuantity(warehouseId, countLine.SkuId, countLine.Quantity);
        
        await _unitOfWork.CommitAsync();
    }
}