using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Filters;

namespace WHMS.Application.Features.Command.Delivery.DeleteDelivery;

public class DeleteDeliveryCommandHnadler : IRequestHandler<DeleteDeliveryCommandRequest, DeleteDeliveryCommandResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDeliveryCommandHnadler(IDeliveryRepository deliveryRepository, IUnitOfWork unitOfWork)
    {
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteDeliveryCommandResponse> Handle(DeleteDeliveryCommandRequest request, CancellationToken cancellationToken)
    {
        var deliveryItems = await _deliveryRepository.GetDeliveryItems(
            new DeliveryItemFilter {DeliveryId = request.DeliveryId},
            false);
        foreach (var item in deliveryItems)
            await _deliveryRepository.DeleteDeliveryItem(item.Id);
        
        await _deliveryRepository.DeleteDelivery(Guid.Parse(request.DeliveryId!));
        await _unitOfWork.CommitAsync();

        return new();
    }
}