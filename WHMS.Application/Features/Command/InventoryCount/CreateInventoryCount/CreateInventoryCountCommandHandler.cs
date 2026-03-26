using MediatR;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.InventoryCount.CreateInventoryCount;

public class CreateInventoryCountCommandHandler : IRequestHandler<CreateInventoryCountCommandRequest, CreateInventoryCountCommandResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateInventoryCountCommandHandler(IInventoryCountRepository inventoryCountRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<CreateInventoryCountCommandResponse> Handle(CreateInventoryCountCommandRequest request, CancellationToken cancellationToken)
    {
        bool isThereUncompletedInventoryCount = await _inventoryCountRepository
            .IsThereUncompletedInventoryCount(Guid.Parse(_currentUserService.WarehouseId!));
        if (isThereUncompletedInventoryCount)
            throw new Exception("There is uncomplated inventory count.");

        Domain.Entities.InventoryCount inventoryCount = new() 
        { 
            WarehouseId = Guid.Parse(_currentUserService.WarehouseId!),
            IsCompleted = false
        };

        await _inventoryCountRepository.CreateInventoryCount(inventoryCount);
        await _unitOfWork.CommitAsync();
        return new();
    }
}