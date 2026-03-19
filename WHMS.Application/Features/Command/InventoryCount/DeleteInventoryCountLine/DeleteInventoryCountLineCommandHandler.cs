using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.InventoryCount.DeleteInventoryCountLine;

public class DeleteInventoryCountLineCommandHandler : IRequestHandler<DeleteInventoryCountLineCommandRequest, DeleteInventoryCountLineCommandResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteInventoryCountLineCommandHandler(IInventoryCountRepository inventoryCountRepository, IUnitOfWork unitOfWork)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteInventoryCountLineCommandResponse> Handle(DeleteInventoryCountLineCommandRequest request, CancellationToken cancellationToken)
    {
        var inventoryCountLine = await _inventoryCountRepository.GetInventoryCountLine(Guid.Parse(request.InventoryCountLineId!));
        if (inventoryCountLine is null)
            throw new Exception("Inventory count line not found.");

        _inventoryCountRepository.DeleteInventoryCountLine(inventoryCountLine);
        await _unitOfWork.CommitAsync();

        return new();
    }
}