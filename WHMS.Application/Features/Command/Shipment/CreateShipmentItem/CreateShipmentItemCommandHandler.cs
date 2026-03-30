using System.Numerics;
using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Shipment.CreateShipmentItem;

public class CreateShipmentItemCommandHandler : IRequestHandler<CreateShipmentItemCommandRequest, CreateShipmentItemCommandResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICatalogRepository _catalogRepository;

    public CreateShipmentItemCommandHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork, ICatalogRepository catalogRepository)
    {
        _shipmentRepository = shipmentRepository;
        _unitOfWork = unitOfWork;
        _catalogRepository = catalogRepository;
    }

    public async Task<CreateShipmentItemCommandResponse> Handle(CreateShipmentItemCommandRequest request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetShipment(Guid.Parse(request.ShipmentId!));
        if (shipment is null)
            throw new Exception("Shipment not found.");
        
        if (shipment.SendingDate is not null)
            throw new Exception("Shipment has alread been sent.");

        var sku = await _catalogRepository.GetSku(Guid.Parse(request.SkuId!));
        if (sku is null)
            throw new Exception("SKU not found.");

        var shipmentItem = new ShipmentItem
        {
            ShipmentId = Guid.Parse(request.ShipmentId!),
            SkuId = Guid.Parse(request.SkuId!),
            Quantity = request.Quantity
        };

        await _shipmentRepository.AddShipmentItem(shipmentItem);
        await _unitOfWork.CommitAsync();

        return new()
        {
            ShipmentItemId = shipmentItem.Id.ToString(),
            SkuId = shipmentItem.SkuId.ToString(),
            SkuName = sku.SKUName,
            Quantity = shipmentItem.Quantity
        };
    }
}