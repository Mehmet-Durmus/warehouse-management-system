using System.Runtime.CompilerServices;
using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Delivery.DeleteDeliveryItem;

public class DeleteDeliveryItemCommandHandler : IRequestHandler<DeleteDeliveryItemCommandRequest, DeleteDeliveryItemCommandResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteDeliveryItemCommandHandler(IDeliveryRepository deliveryRepository, IUnitOfWork unitOfWork)
    {
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteDeliveryItemCommandResponse> Handle(DeleteDeliveryItemCommandRequest request, CancellationToken cancellationToken)
    {
        var deliveryItem = await _deliveryRepository.GetDeliveryItem(Guid.Parse(request.DeliveryItemId!));
        if (deliveryItem is null)
            throw new Exception("Delivery item not found.");
        var delivery = await _deliveryRepository.GetDelivery(deliveryItem.DeliveryId);
        if (delivery is null)
            throw new Exception("Delivery not found.");
        if (delivery.ReceivedAt is not null)
            throw new Exception("Delivery has already been received.");
            
        await _deliveryRepository.DeleteDeliveryItem(Guid.Parse(request.DeliveryItemId!));
        await _unitOfWork.CommitAsync();
        return new();
    }
}