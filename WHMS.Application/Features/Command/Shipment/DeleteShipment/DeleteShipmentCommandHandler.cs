using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Shipment.DeleteShipment;

public class DeleteShipmentCommandHandler : IRequestHandler<DeleteShipmentCommandRequest>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteShipmentCommandHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork)
    {
        _shipmentRepository = shipmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteShipmentCommandRequest request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetShipment(Guid.Parse(request.ShipmentId!));
        if (shipment is null)
            throw new Exception("Shipment not found.");

        if (shipment.SendingDate is not null)
            throw new Exception("Shipment has already been sent.");

        if (shipment.ShipmentItems is not null)
            foreach (var item in shipment.ShipmentItems)
                await _shipmentRepository.DeleteShipmentItem(item.Id);
            
        await _shipmentRepository.DeleteShipment(shipment.Id);
        await _unitOfWork.CommitAsync();
    }
}