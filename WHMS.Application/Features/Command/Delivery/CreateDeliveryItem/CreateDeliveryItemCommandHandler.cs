using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Delivery.CreateDeliveryItem;

public class CreateDeliveryItemCommandHandler : IRequestHandler<CreateDeliveryItemCommandRequest, CreateDeliveryItemCommandResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICatalogRepository _catalogRepository;

    public CreateDeliveryItemCommandHandler(IDeliveryRepository deliveryRepository, IUnitOfWork unitOfWork, ICatalogRepository catalogRepository)
    {
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
        _catalogRepository = catalogRepository;
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
        var sku = await _catalogRepository.GetSku(deliveryItem.SkuId);
        return new()
        {
            DeliveryItemId = deliveryItem.Id.ToString(),
            DeliveryId = deliveryItem.DeliveryId.ToString(),
            SkuId = deliveryItem.SkuId.ToString(),
            SkuName = sku.SKUName,
            Quantity = deliveryItem.Quantity
        };
    }
}