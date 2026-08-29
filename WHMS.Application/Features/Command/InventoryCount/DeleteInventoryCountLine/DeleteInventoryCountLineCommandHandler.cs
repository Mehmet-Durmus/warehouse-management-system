using MediatR;
using Microsoft.EntityFrameworkCore.Metadata;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Domain.BusinessRules.InventoryCount;

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
        bool restrictedToOwnLines = _currentUserService.Roles!.Contains(ApplicationRole.WarehouseStaff);
        var inventoryCountLine = InventoryCountRules.EnsureLineAccessible(
            await _inventoryCountRepository.GetInventoryCountLine(Guid.Parse(request.InventoryCountLineId!)),
            restrictedToOwnLines, _currentUserService.UserId);

        var inventoryCount = InventoryCountRules.EnsureAccessibleForWarehouse(
            await _inventoryCountRepository.GetInventoryCount(inventoryCountLine.InventoryCountId),
            Guid.Parse(_currentUserService.WarehouseId!));
        InventoryCountRules.EnsureNotCompleted(inventoryCount.IsCompleted);

        _inventoryCountRepository.DeleteInventoryCountLine(inventoryCountLine);
        await _unitOfWork.CommitAsync();

    }
}