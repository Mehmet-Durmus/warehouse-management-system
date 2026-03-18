using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Shipment.UpdateShipmentItem;

public class UpdateShipmentItemCommandHandler : IRequestHandler<UpdateShipmentItemCommandRequest, UpdateShipmentItemCommandResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateShipmentItemCommandHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork)
    {
        _shipmentRepository = shipmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateShipmentItemCommandResponse> Handle(UpdateShipmentItemCommandRequest request, CancellationToken cancellationToken)
    {
        var shipmentItem = await _shipmentRepository.GetShipmentItem(Guid.Parse(request.ShipmentItemId!));
        if (shipmentItem is null)
            throw new Exception("Shipment item not found.");
        
        shipmentItem.ShipmentId = Guid.Parse(request.ShipmentId!);
        shipmentItem.SkuId = Guid.Parse(request.SkuId!);
        shipmentItem.Quantity = request.Quantity;

        _shipmentRepository.UpdateShipmentItem(shipmentItem);
        await _unitOfWork.CommitAsync();

        return new();
    }
}