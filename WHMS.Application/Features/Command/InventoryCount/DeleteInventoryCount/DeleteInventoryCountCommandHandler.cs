using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.InventoryCount;

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
        var inventoryCount = InventoryCountRules.EnsureAccessibleForWarehouse(
            await _inventoryCountRepository.GetInventoryCount(Guid.Parse(request.InventoryCountId!)),
            Guid.Parse(_currentUserService.WarehouseId!));
        InventoryCountRules.EnsureNotCompleted(inventoryCount.IsCompleted);

        _inventoryCountRepository.DeleteInventoryCount(inventoryCount);

        if (inventoryCount.InventoryCountLines is not null)
            foreach (var inventoryCountLine in inventoryCount.InventoryCountLines)
                _inventoryCountRepository.DeleteInventoryCountLine(inventoryCountLine);

        await _unitOfWork.CommitAsync();

    }
}