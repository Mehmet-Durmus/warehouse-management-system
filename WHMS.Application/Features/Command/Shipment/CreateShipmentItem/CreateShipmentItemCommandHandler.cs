using System.Numerics;
using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Shipment.CreateShipmentItem;

public class CreateShipmentItemCommandHandler : IRequestHandler<CreateShipmentItemCommandRequest, CreateShipmentItemCommandResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateShipmentItemCommandHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork)
    {
        _shipmentRepository = shipmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateShipmentItemCommandResponse> Handle(CreateShipmentItemCommandRequest request, CancellationToken cancellationToken)
    {
        var shipmentItem = new ShipmentItem
        {
            ShipmentId = Guid.Parse(request.ShipmentId!),
            SkuId = Guid.Parse(request.SkuId!),
            Quantity = request.Quantity
        };

        await _shipmentRepository.AddShipmentItem(shipmentItem);
        await _unitOfWork.CommitAsync();

        return new()
        {
            ShipmentItemId = shipmentItem.Id.ToString(),
            ShipmentId = shipmentItem.ShipmentId.ToString(),
            SkuId = shipmentItem.SkuId.ToString(),
            Quantity = shipmentItem.Quantity
        };
    }
}