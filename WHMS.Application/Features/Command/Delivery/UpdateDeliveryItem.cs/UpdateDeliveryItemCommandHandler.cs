using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Delivery.UpdateDeliveryItem;

public class UpdateDeliveryItemCommandHandler : IRequestHandler<UpdateDeliveryItemCommandRequest, UpdateDeliveryItemCommandResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDeliveryItemCommandHandler(IDeliveryRepository deliveryRepository, IUnitOfWork unitOfWork)
    {
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateDeliveryItemCommandResponse> Handle(UpdateDeliveryItemCommandRequest request, CancellationToken cancellationToken)
    {
        var deliveryItem = await _deliveryRepository.GetDeliveryItem(Guid.Parse(request.DeliveryItemId!));
        if (deliveryItem is null)
            throw new Exception("Delivery item not found.");
        var delivery = await _deliveryRepository.GetDelivery(deliveryItem.DeliveryId);
        if (delivery is null)
            throw new Exception("Delivery not found.");
        if (delivery.ReceivedAt is not null)
            throw new Exception("Delivery has already been received.");

        deliveryItem.DeliveryId = Guid.Parse(request.DeliveryId!);
        deliveryItem.SkuId = Guid.Parse(request.SkuId!);
        deliveryItem.Quantity = request.Quantity;
        
        await _unitOfWork.CommitAsync();
        return new();
    }
}