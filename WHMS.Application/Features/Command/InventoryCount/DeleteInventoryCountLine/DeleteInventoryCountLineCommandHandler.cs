using MediatR;
using Microsoft.EntityFrameworkCore.Metadata;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.InventoryCount.DeleteInventoryCountLine;

public class DeleteInventoryCountLineCommandHandler : IRequestHandler<DeleteInventoryCountLineCommandRequest>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteInventoryCountLineCommandHandler(IInventoryCountRepository inventoryCountRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteInventoryCountLineCommandRequest request, CancellationToken cancellationToken)
    {
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

        var inventoryCount = await _inventoryCountRepository.GetInventoryCount(inventoryCountLine!.InventoryCountId);
        bool notFoundInventoryCount = inventoryCount switch
        {
            null => true,
            not null when inventoryCount.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            _ => false
        };

        if (notFoundInventoryCount)
            throw new Exception("Inventory count not found.");

        if (inventoryCount!.IsCompleted)
            throw new Exception("Inventory count has already been completed.");

        _inventoryCountRepository.DeleteInventoryCountLine(inventoryCountLine!);
        await _unitOfWork.CommitAsync();

    }
}