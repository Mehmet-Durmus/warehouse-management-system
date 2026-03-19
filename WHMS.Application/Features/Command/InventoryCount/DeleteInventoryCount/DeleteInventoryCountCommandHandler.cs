using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.InventoryCount.DeleteInventoryCount;

public class DeleteInventoryCountCommandHandler : IRequestHandler<DeleteInventoryCountCommandRequest, DeleteInventoryCountCommandResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteInventoryCountCommandHandler(IInventoryCountRepository inventoryCountRepository, IUnitOfWork unitOfWork)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteInventoryCountCommandResponse> Handle(DeleteInventoryCountCommandRequest request, CancellationToken cancellationToken)
    {
        var inventoryCount = await _inventoryCountRepository.GetInventoryCount(Guid.Parse(request.InventoryCountId!));
        if (inventoryCount is null)
            throw new Exception("Inventory count not found.");
        
        _inventoryCountRepository.DeleteInventoryCount(inventoryCount);

        if (inventoryCount.InventoryCountLines is not null)
            foreach (var inventoryCountLine in inventoryCount.InventoryCountLines)
                _inventoryCountRepository.DeleteInventoryCountLine(inventoryCountLine);

        await _unitOfWork.CommitAsync();

        return new();
    }
}