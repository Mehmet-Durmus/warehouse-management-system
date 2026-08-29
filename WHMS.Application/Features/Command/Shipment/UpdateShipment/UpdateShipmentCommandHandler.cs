using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.Shipment;
using WHMS.Domain.BusinessRules.Store;
using WHMS.Domain.BusinessRules.Warehouse;

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
        var shipment = ShipmentRules.EnsureExists(await _shipmentRepository.GetShipment(Guid.Parse(request.ShipmentId!)));
        ShipmentRules.EnsureNotSent(shipment.SendingDate is not null);

        WarehouseRules.EnsureExists(await _warehouseRepository.GetWarehouse(Guid.Parse(request.WarehouseId!)));
        StoreRules.EnsureExists(await _storeRepository.GetStore(Guid.Parse(request.StoreId!)));

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