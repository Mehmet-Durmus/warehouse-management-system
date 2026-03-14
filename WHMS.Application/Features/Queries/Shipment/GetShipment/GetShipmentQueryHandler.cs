using System.Runtime.Serialization;
using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.DTOs.Catalog;
using WHMS.Application.DTOs.Shipment;

namespace WHMS.Application.Features.Queries.Shipment.GetShipment;

public class GetShipmentQueryHandler : IRequestHandler<GetShipmentQueryRequest, GetShipmentQueryResponse>
{
    private readonly IShipmentRepository _shipmentRepository;

    public GetShipmentQueryHandler(IShipmentRepository shipmentRepository)
    {
        _shipmentRepository = shipmentRepository;
    }

    public async Task<GetShipmentQueryResponse> Handle(GetShipmentQueryRequest request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetShipment(Guid.Parse(request.ShipmentId!));
        if (shipment is null)
            throw new Exception("Shipment not found.");

        GetShipmentQueryResponse response = new()
        {
            WarehouseId = shipment.WarehouseId.ToString(),
            StoreId = shipment.StoreId.ToString(),
            ExpectedSendingDate = shipment.ExpectedSendingDate,
            SendingDate = shipment.SendingDate,
            SentById = shipment.SentById.ToString(),
            ShipmentItems = [],
            CreatedAt = shipment.CreatedAt,
            CreatedById = shipment.CreatedById.ToString(),
            CreatedByName = shipment.CreatedByName,
            CreatedByUserName = shipment.CreatedByUserName,
            UpdatedAt = shipment.UpdatedAt,
            UpdatedById = shipment.UpdatedById.ToString(),
            UpdatedByName = shipment.UpdatedByName,
            UpdatedByUserName = shipment.UpdatedByUserName,
        };

        if (shipment.ShipmentItems is not null)
            foreach (var item in shipment.ShipmentItems)
                response.ShipmentItems.Add(new ShipmentItemDto 
                {  
                    ShipmentItemId = item.Id.ToString(),
                    Sku = new SkuDto
                    {
                        Id = item.SkuId.ToString(),
                        SKUName = item.SKU!.SKUName,
                        Barcode = item.SKU.Barcode,
                        UnitPrice = item.SKU.UnitPrice
                    },
                    Quantity = item.Quantity
                });

        return response;
    }
}