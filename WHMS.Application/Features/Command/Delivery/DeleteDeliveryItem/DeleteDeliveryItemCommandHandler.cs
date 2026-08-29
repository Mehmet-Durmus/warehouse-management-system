using System.Runtime.CompilerServices;
using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.Delivery;

namespace WHMS.Application.Features.Command.Delivery.DeleteDeliveryItem;

public class DeleteDeliveryItemCommandHandler : IRequestHandler<DeleteDeliveryItemCommandRequest>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteDeliveryItemCommandHandler(IDeliveryRepository deliveryRepository, IUnitOfWork unitOfWork)
    {
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteDeliveryItemCommandRequest request, CancellationToken cancellationToken)
    {
        var deliveryItem = DeliveryRules.EnsureItemExists(await _deliveryRepository.GetDeliveryItem(Guid.Parse(request.DeliveryItemId!)));
        var delivery = DeliveryRules.EnsureExists(await _deliveryRepository.GetDelivery(deliveryItem.DeliveryId));
        DeliveryRules.EnsureNotReceived(delivery.ReceivedAt is not null);
            
        await _deliveryRepository.DeleteDeliveryItem(Guid.Parse(request.DeliveryItemId!));
        await _unitOfWork.CommitAsync();
    }
}