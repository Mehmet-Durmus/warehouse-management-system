using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;

namespace WHMS.Application.Features.Command.Store.DeleteStore;

public class DeleteStoreCommandHandler : IRequestHandler<DeleteStoreCommandRequest>
{
    private readonly IStoreRepository _storeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShipmentRepository _shipmentRepository;

    public DeleteStoreCommandHandler(IStoreRepository storeRepository, IUnitOfWork unitOfWork, IShipmentRepository shipmentRepository)
    {
        _storeRepository = storeRepository;
        _unitOfWork = unitOfWork;
        _shipmentRepository = shipmentRepository;
    }

    public async Task Handle(DeleteStoreCommandRequest request, CancellationToken cancellationToken)
    {
        ShipmentFilter filter = new()
        {
            StoreId = request.StoreId,
            IsSent = false
        };
        var shipments = await _shipmentRepository.GetShipments(filter, applyPagination: false);
        foreach (var shipment in shipments)
            shipment.IsActive = false;

        await _storeRepository.SoftDelete(Guid.Parse(request.StoreId!));
        await _unitOfWork.CommitAsync();
    }
}