using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.InventoryCount.CreateInventoryCount;

public class CreateInventoryCountCommandHandler : IRequestHandler<CreateInventoryCountCommandRequest, CreateInventoryCountCommandResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventoryCountCommandHandler(IInventoryCountRepository inventoryCountRepository, IUnitOfWork unitOfWork)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateInventoryCountCommandResponse> Handle(CreateInventoryCountCommandRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.InventoryCount inventoryCount = new() { WarehouseId = Guid.Parse(request.WarehouseId!)};

        await _inventoryCountRepository.CreateInventoryCount(inventoryCount);
        await _unitOfWork.CommitAsync();
        return new();
    }
}