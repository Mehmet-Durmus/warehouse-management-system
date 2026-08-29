using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.Store;
using WHMS.Domain.BusinessRules.Warehouse;

namespace WHMS.Application.Features.Command.Shipment.CreateShipment;

public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommandRequest, CreateShipmentCommandResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IStoreRepository _storeRepository;

    public CreateShipmentCommandHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork, IWarehouseRepository warehouseRepository, IStoreRepository storeRepository)
    {
        _shipmentRepository = shipmentRepository;
        _unitOfWork = unitOfWork;
        _warehouseRepository = warehouseRepository;
        _storeRepository = storeRepository;
    }

    public async Task<CreateShipmentCommandResponse> Handle(CreateShipmentCommandRequest request, CancellationToken cancellationToken)
    {
        WarehouseRules.EnsureExists(await _warehouseRepository.GetWarehouse(Guid.Parse(request.WarehouseId!)));
        StoreRules.EnsureExists(await _storeRepository.GetStore(Guid.Parse(request.StoreId!)));

        var shipment = new Domain.Entities.Shipment
        {
            WarehouseId = Guid.Parse(request.WarehouseId!),
            StoreId = Guid.Parse(request.StoreId!),
            ExpectedSendingDate = request.ExpectedSendingDate
        };

        await _shipmentRepository.CreateShipment(shipment);
        await _unitOfWork.CommitAsync();

        return new()
        {
            ShipmentId = shipment.Id.ToString(),
            WarehouseId = shipment.WarehouseId.ToString(),
            StoreId = shipment.StoreId.ToString(),
            ExpectedSendingDate = shipment.ExpectedSendingDate
        };
    }
}