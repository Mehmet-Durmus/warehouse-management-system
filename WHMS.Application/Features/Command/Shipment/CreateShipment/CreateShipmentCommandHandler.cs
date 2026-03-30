using MediatR;
using WHMS.Application.Abstractions.Persistence;

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
        var warehouse = await _warehouseRepository.GetWarehouse(Guid.Parse(request.WarehouseId!))!;
        if (warehouse is null)
            throw new Exception("Warehouse not found");

        var store = await _storeRepository.GetStore(Guid.Parse(request.StoreId!));
        if (store is null)
            throw new Exception("Store not found.");

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