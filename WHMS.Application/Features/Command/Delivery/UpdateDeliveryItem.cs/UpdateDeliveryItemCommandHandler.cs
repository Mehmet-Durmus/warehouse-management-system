using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.Catalog;
using WHMS.Domain.BusinessRules.Delivery;

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
        var deliveryItem = DeliveryRules.EnsureItemExists(await _deliveryRepository.GetDeliveryItem(Guid.Parse(request.DeliveryItemId!)));
        var delivery = DeliveryRules.EnsureExists(await _deliveryRepository.GetDelivery(deliveryItem.DeliveryId));
        DeliveryRules.EnsureNotReceived(delivery.ReceivedAt is not null);

        var sku = SkuRules.EnsureExists(await _catalogRepository.GetSku(Guid.Parse(request.SkuId!)));

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