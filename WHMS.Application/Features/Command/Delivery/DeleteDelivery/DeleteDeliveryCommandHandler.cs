using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;

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
        var delivery = await _deliveryRepository.GetDelivery(Guid.Parse(request.DeliveryId!));
        if (delivery is null)
            throw new Exception("Delivery not found.");
        if (delivery.ReceivedAt is not null)
            throw new Exception("Delivery has already been received.");

        var deliveryItems = await _deliveryRepository.GetDeliveryItems(
            new DeliveryItemFilter {DeliveryId = request.DeliveryId},
            false);
        foreach (var item in deliveryItems)
            await _deliveryRepository.DeleteDeliveryItem(item.Id);
        
        await _deliveryRepository.DeleteDelivery(Guid.Parse(request.DeliveryId!));
        await _unitOfWork.CommitAsync();
    }
}