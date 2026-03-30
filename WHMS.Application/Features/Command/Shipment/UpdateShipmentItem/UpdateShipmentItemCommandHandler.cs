using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;

namespace WHMS.Application.Features.Command.Shipment.UpdateShipmentItem;

public class UpdateShipmentItemCommandHandler : IRequestHandler<UpdateShipmentItemCommandRequest, UpdateShipmentItemCommandResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICatalogRepository _catalogRepository;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IStockStateRepository _stockStateRepository;

    public UpdateShipmentItemCommandHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork, ICatalogRepository catalogRepository, IDeliveryRepository deliveryRepository, IStockStateRepository stockStateRepository)
    {
        _shipmentRepository = shipmentRepository;
        _unitOfWork = unitOfWork;
        _catalogRepository = catalogRepository;
        _deliveryRepository = deliveryRepository;
        _stockStateRepository = stockStateRepository;
    }

    public async Task<UpdateShipmentItemCommandResponse> Handle(UpdateShipmentItemCommandRequest request, CancellationToken cancellationToken)
    {
        var shipmentItem = await _shipmentRepository.GetShipmentItem(Guid.Parse(request.ShipmentItemId!));
        if (shipmentItem is null)
            throw new Exception("Shipment item not found.");

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
        
        shipmentItem.ShipmentId = Guid.Parse(request.ShipmentId!);
        shipmentItem.SkuId = Guid.Parse(request.SkuId!);
        shipmentItem.Quantity = request.Quantity;

        _shipmentRepository.UpdateShipmentItem(shipmentItem);
        await _unitOfWork.CommitAsync();

        return new()
        {
            SkuId = shipmentItem.SkuId.ToString(),
            SkuName = sku.SKUName,
            Quantity = shipmentItem.Quantity
        };
    }
}