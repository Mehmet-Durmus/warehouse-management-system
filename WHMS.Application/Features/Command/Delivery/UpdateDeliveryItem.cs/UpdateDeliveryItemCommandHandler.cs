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
        deliveryItem.DeliveryId = Guid.Parse(request.DeliveryId!);
        deliveryItem.SkuId = Guid.Parse(request.SkuId!);
        deliveryItem.Quantity = request.Quantity;
        _deliveryRepository.UpdateDeliveryItem(deliveryItem);
        await _unitOfWork.CommitAsync();
        return new();
    }
}