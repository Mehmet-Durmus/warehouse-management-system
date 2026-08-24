using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Shipment.DeleteShipmentItem;

public class DeleteShipmentItemCommandHandler : IRequestHandler<DeleteShipmentItemCommandRequest>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteShipmentItemCommandHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork)
    {
        _shipmentRepository = shipmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteShipmentItemCommandRequest request, CancellationToken cancellationToken)
    {
        var shipmentItem = await _shipmentRepository.GetShipmentItem(Guid.Parse(request.ShipmentItemId!));
        if (shipmentItem is null)
            throw new Exception("Shipment item not found.");

        var shipment = await _shipmentRepository.GetShipment(shipmentItem.ShipmentId);
        if (shipment is null)
            throw new Exception("Shipment not found.");

        if (shipment.SendingDate is not null)
            throw new Exception("Shipment has already been sent.");

        await _shipmentRepository.DeleteShipmentItem(Guid.Parse(request.ShipmentItemId!));
        await _unitOfWork.CommitAsync();
    }
}