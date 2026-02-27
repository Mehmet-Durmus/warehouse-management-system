using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Warehouse.DeleteWarehouse;

public class DeleteWarehouseCommandHnadler : IRequestHandler<DeleteWarehouseCommandRequest, DeleteWarehouseCommandResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWarehouseCommandHnadler(IWarehouseRepository warehouseRepository, IUnitOfWork unitOfWork)
    {
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteWarehouseCommandResponse> Handle(DeleteWarehouseCommandRequest request, CancellationToken cancellationToken)
    {
        await _warehouseRepository.SoftDelete(Guid.Parse(request.WarehouseId));
        await _unitOfWork.CommitAsync();
        return new();
    }
}