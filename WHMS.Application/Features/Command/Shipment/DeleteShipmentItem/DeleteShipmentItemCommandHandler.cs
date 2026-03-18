using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Shipment.DeleteShipmentItem;

public class DeleteShipmentItemCommandHandler : IRequestHandler<DeleteShipmentItemCommandRequest, DeleteShipmentItemCommandResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteShipmentItemCommandHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork)
    {
        _shipmentRepository = shipmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteShipmentItemCommandResponse> Handle(DeleteShipmentItemCommandRequest request, CancellationToken cancellationToken)
    {
        await _shipmentRepository.DeleteShipmentItem(Guid.Parse(request.ShipmentItemId!));
        await _unitOfWork.CommitAsync();

        return new();
    }
}