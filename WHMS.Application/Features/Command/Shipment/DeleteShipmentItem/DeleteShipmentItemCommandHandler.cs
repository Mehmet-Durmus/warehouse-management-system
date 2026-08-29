using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.Shipment;

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
        var shipmentItem = ShipmentRules.EnsureItemExists(await _shipmentRepository.GetShipmentItem(Guid.Parse(request.ShipmentItemId!)));

        var shipment = ShipmentRules.EnsureExists(await _shipmentRepository.GetShipment(shipmentItem.ShipmentId));
        ShipmentRules.EnsureNotSent(shipment.SendingDate is not null);

        await _shipmentRepository.DeleteShipmentItem(Guid.Parse(request.ShipmentItemId!));
        await _unitOfWork.CommitAsync();
    }
}