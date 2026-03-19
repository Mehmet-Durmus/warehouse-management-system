using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.InventoryCount.UpdateInventoryCountLine;

public class UpdateInventoryCountLineCommandHandler : IRequestHandler<UpdateInventoryCountLineCommandRequest, UpdateInventoryCountLineCommandResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInventoryCountLineCommandHandler(IInventoryCountRepository inventoryCountRepository, IUnitOfWork unitOfWork)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateInventoryCountLineCommandResponse> Handle(UpdateInventoryCountLineCommandRequest request, CancellationToken cancellationToken)
    {
        var inventoryCountLine = await _inventoryCountRepository.GetInventoryCountLine(Guid.Parse(request.InventoryCountLineId!));
        if (inventoryCountLine is null)
            throw new Exception("Inventory count line not found.");

        inventoryCountLine.InventoryCountId = Guid.Parse(request.InventoryCountId!);
        inventoryCountLine.SkuId = Guid.Parse(request.SkuId!);
        inventoryCountLine.Quantity = request.Quantity;
        
        await _unitOfWork.CommitAsync();
        return new();
    }
}