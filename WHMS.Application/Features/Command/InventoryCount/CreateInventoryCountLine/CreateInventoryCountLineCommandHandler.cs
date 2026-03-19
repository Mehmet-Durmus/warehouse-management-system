using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.InventoryCount.CreateInventoryCountLine;

public class CreateInventoryCountLineCommandHandler : IRequestHandler<CreateInventoryCountLineCommandRequest, CreateInventoryCountLineCommandResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventoryCountLineCommandHandler(IInventoryCountRepository inventoryCountRepository, IUnitOfWork unitOfWork)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateInventoryCountLineCommandResponse> Handle(CreateInventoryCountLineCommandRequest request, CancellationToken cancellationToken)
    {
        InventoryCountLine inventoryCountLine = new()
        {
            InventoryCountId = Guid.Parse(request.InventoryCountId!),
            SkuId = Guid.Parse(request.SkuId!),
            Quantity = request.Quantity
        };
        await _inventoryCountRepository.CreateInventoryCounLine(inventoryCountLine);
        await _unitOfWork.CommitAsync();
        return new();
    }
}