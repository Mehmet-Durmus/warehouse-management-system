using System.Numerics;
using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Shipment.CreateShipmentItem;

public class CreateShipmentItemCommandHandler : IRequestHandler<CreateShipmentItemCommandRequest, CreateShipmentItemCommandResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICatalogRepository _catalogRepository;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IStockStateRepository _stockStateRepository;

    public CreateShipmentItemCommandHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork, ICatalogRepository catalogRepository, IDeliveryRepository deliveryRepository, IStockStateRepository stockStateRepository)
    {
        _shipmentRepository = shipmentRepository;
        _unitOfWork = unitOfWork;
        _catalogRepository = catalogRepository;
        _deliveryRepository = deliveryRepository;
        _stockStateRepository = stockStateRepository;
    }

    public async Task<CreateShipmentItemCommandResponse> Handle(CreateShipmentItemCommandRequest request, CancellationToken cancellationToken)
    {
        


        var shipment = await _shipmentRepository.GetShipment(Guid.Parse(request.ShipmentId!));
        if (shipment is null)
            throw new Exception("Shipment not found.");
        
        if (shipment.SendingDate is not null)
            throw new Exception("Shipment has alread been sent.");

        int stock = await _stockStateRepository.GetStockQuantity(shipment.WarehouseId, Guid.Parse(request.SkuId!));
        List<string> skuId = new() {request.SkuId!};
        DeliveryFilter deliveryFilter = new()
        {
            WarehouseId = shipment.WarehouseId.ToString(),
            SkuIds = skuId,
            ExpectedArrivalBefore = shipment.ExpectedSendingDate
        };
        var deliveries = await _deliveryRepository.GetDeliveries(deliveryFilter, applyPagination: false);
        int deliveryQuantity = deliveries
            .SelectMany(d => d.DeliveryItems ?? [])
            .Where(di => di.SkuId == Guid.Parse(request.SkuId!))
            .Sum(di => di.Quantity);
        ShipmentFilter shipmentFilter = new()
        {
            WarehouseId = shipment.WarehouseId.ToString(),
            SkuIds = skuId,
            ExpectedSendingDateBefore = shipment.ExpectedSendingDate
        };
        var shipments = await _shipmentRepository.GetShipments(shipmentFilter, applyPagination: false);
        int shipmentQuantity = shipments
            .SelectMany(s => s.ShipmentItems ?? [])
            .Where(si => si.SkuId == Guid.Parse(request.SkuId!))
            .Sum(si => si.Quantity);            
        int expectedStock = stock + deliveryQuantity - shipmentQuantity;

        if (expectedStock < request.Quantity)
            throw new Exception("Insufficient stock in warehouse for the scheduled shipment date.");
        
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