using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.InventoryCount.DeleteInventoryCount;

public class DeleteInventoryCountCommandHandler : IRequestHandler<DeleteInventoryCountCommandRequest>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteInventoryCountCommandHandler(IInventoryCountRepository inventoryCountRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteInventoryCountCommandRequest request, CancellationToken cancellationToken)
    {
        var inventoryCount = await _inventoryCountRepository.GetInventoryCount(Guid.Parse(request.InventoryCountId!));
        bool notFound = inventoryCount switch
        {
            null => true,
            not null when inventoryCount.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            _ => false
        };
        
        if (notFound)
            throw new Exception("Inventory count not found.");

        if (inventoryCount!.IsCompleted)
            throw new Exception("Inventory count has already been completed.");
        
        _inventoryCountRepository.DeleteInventoryCount(inventoryCount!);

        if (inventoryCount!.InventoryCountLines is not null)
            foreach (var inventoryCountLine in inventoryCount.InventoryCountLines)
                _inventoryCountRepository.DeleteInventoryCountLine(inventoryCountLine);

        await _unitOfWork.CommitAsync();

    }
}