using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Shipment.CreateShipment;

public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommandRequest, CreateShipmentCommandResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateShipmentCommandHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork)
    {
        _shipmentRepository = shipmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateShipmentCommandResponse> Handle(CreateShipmentCommandRequest request, CancellationToken cancellationToken)
    {
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