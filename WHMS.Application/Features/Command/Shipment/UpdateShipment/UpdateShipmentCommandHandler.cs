using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Shipment.UpdateShipment;

public class UpdateShipmentCommandHandler : IRequestHandler<UpdateShipmentCommandRequest, UpdateShipmentCommandResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IStoreRepository _storeRepository;

    public UpdateShipmentCommandHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork, IWarehouseRepository warehouseRepository, IStoreRepository storeRepository)
    {
        _shipmentRepository = shipmentRepository;
        _unitOfWork = unitOfWork;
        _warehouseRepository = warehouseRepository;
        _storeRepository = storeRepository;
    }

    public async Task<UpdateShipmentCommandResponse> Handle(UpdateShipmentCommandRequest request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetShipment(Guid.Parse(request.ShipmentId!));
        if (shipment is null)
            throw new Exception("Shipment not found.");

        var warehouse = await _warehouseRepository.GetWarehouse(Guid.Parse(request.WarehouseId!))!;
        if (warehouse is null)
            throw new Exception("Warehouse not found.");

        var store = await _storeRepository.GetStore(Guid.Parse(request.StoreId!));
        if (store is null)
            throw new Exception("Store not found.");

        shipment.WarehouseId = Guid.Parse(request.WarehouseId!);
        shipment.StoreId = Guid.Parse(request.StoreId!);
        shipment.ExpectedSendingDate = request.ExpectedSendingDate;

        _shipmentRepository.UpdateShipment(shipment);
        await _unitOfWork.CommitAsync();

        return new()
        {
            WarehouseId = shipment.WarehouseId.ToString(),
            StoreId = shipment.StoreId.ToString(),
            ExpectedSendingDate = shipment.ExpectedSendingDate
        };
    }
}