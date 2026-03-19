using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.InventoryCount.UpdateInventoryCount;

public class UpdateInventoryCountCommandHandler : IRequestHandler<UpdateInventoryCountCommandRequest, UpdateInventoryCountCommandResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInventoryCountCommandHandler(IInventoryCountRepository inventoryCountRepository, IUnitOfWork unitOfWork)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateInventoryCountCommandResponse> Handle(UpdateInventoryCountCommandRequest request, CancellationToken cancellationToken)
    {
        var inventoryCount = await _inventoryCountRepository.GetInventoryCount(Guid.Parse(request.InventoryCountId!));
        if (inventoryCount is null)
            throw new Exception("Inventory count not found.");

        inventoryCount.WarehouseId = Guid.Parse(request.WarehouseId!);
        await _unitOfWork.CommitAsync();
        return new();
    }
}