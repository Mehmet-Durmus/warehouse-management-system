using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Delivery.UpdateDelivery;

public class UpdateDeliveryCommandHandler : IRequestHandler<UpdateDeliveryCommandRequest, UpdateDeliveryCommandResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDeliveryCommandHandler(IDeliveryRepository deliveryRepository, IUnitOfWork unitOfWork)
    {
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateDeliveryCommandResponse> Handle(UpdateDeliveryCommandRequest request, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryRepository.GetDelivery(Guid.Parse(request.DeliveryId!));
        delivery.WarehouseId = Guid.Parse(request.WarehouseId!);
        delivery.ExpectedArrivalDate = request.ExpectedArrivalDate;
        _deliveryRepository.UpdateDelivery(delivery);
        await _unitOfWork.CommitAsync();
        return new();
    }
}