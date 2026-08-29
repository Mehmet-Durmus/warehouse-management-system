using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.BusinessRules.Delivery;

namespace WHMS.Application.Features.Command.Delivery.DeleteDelivery;

public class DeleteDeliveryCommandHnadler : IRequestHandler<DeleteDeliveryCommandRequest>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDeliveryCommandHnadler(IDeliveryRepository deliveryRepository, IUnitOfWork unitOfWork)
    {
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteDeliveryCommandRequest request, CancellationToken cancellationToken)
    {
        var delivery = DeliveryRules.EnsureExists(await _deliveryRepository.GetDelivery(Guid.Parse(request.DeliveryId!)));
        DeliveryRules.EnsureNotReceived(delivery.ReceivedAt is not null);

        var deliveryItems = await _deliveryRepository.GetDeliveryItems(
            new DeliveryItemFilter {DeliveryId = request.DeliveryId},
            false);
        foreach (var item in deliveryItems)
            await _deliveryRepository.DeleteDeliveryItem(item.Id);
        
        await _deliveryRepository.DeleteDelivery(Guid.Parse(request.DeliveryId!));
        await _unitOfWork.CommitAsync();
    }
}