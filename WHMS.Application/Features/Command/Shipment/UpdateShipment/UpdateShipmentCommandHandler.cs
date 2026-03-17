using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Shipment.UpdateShipment;

public class UpdateShipmentCommandHandler : IRequestHandler<UpdateShipmentCommandRequest, UpdateShipmentCommandResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateShipmentCommandHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork)
    {
        _shipmentRepository = shipmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateShipmentCommandResponse> Handle(UpdateShipmentCommandRequest request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetShipment(Guid.Parse(request.ShipmentId!));
        if (shipment is null)
            throw new Exception("Shipment not found.");

        shipment.WarehouseId = Guid.Parse(request.WarehouseId!);
        shipment.StoreId = Guid.Parse(request.StoreId!);
        shipment.ExpectedSendingDate = request.ExpectedSendingDate;

        _shipmentRepository.UpdateShipment(shipment);
        await _unitOfWork.CommitAsync();

        return new();
    }
}