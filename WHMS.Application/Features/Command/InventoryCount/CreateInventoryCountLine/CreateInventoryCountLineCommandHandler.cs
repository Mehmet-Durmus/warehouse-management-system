using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
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
        var inventoryCount = await _inventoryCountRepository.GetInventoryCount(Guid.Parse(request.InventoryCountId!));
        if (inventoryCount is null || warehouseId != inventoryCount.WarehouseId)
            throw new Exception("Inventory count not found.");
        
        if (inventoryCount.IsCompleted)
            throw new Exception("This inventory count already completed.");

        int currentStock = await _stockStateRepository.GetStockQuantity(warehouseId, Guid.Parse(request.SkuId!));
        
        InventoryCountLine inventoryCountLine = new()
        {
            InventoryCountId = Guid.Parse(request.InventoryCountId!),
            SkuId = Guid.Parse(request.SkuId!),
            Quantity = request.Quantity,
            Variance = request.Quantity - currentStock
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