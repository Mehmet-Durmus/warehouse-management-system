using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Delivery.CreateDeliveryItem;

public class CreateDeliveryItemCommandHandler : IRequestHandler<CreateDeliveryItemCommandRequest, CreateDeliveryItemCommandResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDeliveryItemCommandHandler(IDeliveryRepository deliveryRepository, IUnitOfWork unitOfWork)
    {
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateDeliveryItemCommandResponse> Handle(CreateDeliveryItemCommandRequest request, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryRepository.GetDelivery(Guid.Parse(request.DeliveryId!));
        if (delivery is null)
            throw new Exception("Delivery not found.");
        
        if (delivery.ReceivedAt is not null)
            throw new Exception("Delivery has already been received.");
        
        var deliveryItem = new DeliveryItem
        {
            DeliveryId = Guid.Parse(request.DeliveryId!),
            SkuId = Guid.Parse(request.SkuId!),
            Quantity = request.Quantity
        };
        await _deliveryRepository.AddDeliveryItem(deliveryItem);
        await _unitOfWork.CommitAsync();
        return new()
        {
            Id = deliveryItem.Id.ToString(),
            DeliveryId = deliveryItem.DeliveryId.ToString(),
            SkuId = deliveryItem.SkuId.ToString(),
            Quantity = deliveryItem.Quantity
        };
    }
}