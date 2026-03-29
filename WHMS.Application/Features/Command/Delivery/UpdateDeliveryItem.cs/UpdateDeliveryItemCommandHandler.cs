using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Delivery.UpdateDeliveryItem;

public class UpdateDeliveryItemCommandHandler : IRequestHandler<UpdateDeliveryItemCommandRequest, UpdateDeliveryItemCommandResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICatalogRepository _catalogRepository;

    public UpdateDeliveryItemCommandHandler(IDeliveryRepository deliveryRepository, IUnitOfWork unitOfWork, ICatalogRepository catalogRepository)
    {
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
        _catalogRepository = catalogRepository;
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

        var sku = await _catalogRepository.GetSku(Guid.Parse(request.SkuId!));
        if (sku is null)
            throw new Exception("Sku not found.");

        deliveryItem.DeliveryId = Guid.Parse(request.DeliveryId!);
        deliveryItem.SkuId = Guid.Parse(request.SkuId!);
        deliveryItem.Quantity = request.Quantity;
        
        await _unitOfWork.CommitAsync();
        return new()
        {
            DeliveryId = deliveryItem.DeliveryId.ToString(),
            SkuId = deliveryItem.SkuId.ToString(),
            SkuName = sku.SKUName,
            Quantity = deliveryItem.Quantity
        };
    }
}