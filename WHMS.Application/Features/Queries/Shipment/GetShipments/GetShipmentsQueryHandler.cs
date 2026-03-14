using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.DTOs.Catalog;
using WHMS.Application.DTOs.Shipment;
using WHMS.Application.Filters;

namespace WHMS.Application.Features.Queries.Shipment.GetShipments;

public class GetShipmentsQueryHandler : IRequestHandler<GetShipmentsQueryRequest, GetShipmentsQueryResponse>
{
    private readonly IShipmentRepository _shipmentRepository;

    public GetShipmentsQueryHandler(IShipmentRepository shipmentRepository)
    {
        _shipmentRepository = shipmentRepository;
    }

    public async Task<GetShipmentsQueryResponse> Handle(GetShipmentsQueryRequest request, CancellationToken cancellationToken)
    {
        ShipmentFilter filter = new()
        {
            WarehouseId = request.WarehouseId,
            WarehouseCityId = request.WarehouseCityId,
            WarehouseDistrictId = request.WarehouseDistrictId,
            StoreId = request.StoreId,
            StoreCityId = request.StoreCityId,
            StoreDistrictId = request.StoreDistrictId,
            IsSent = request.IsSent,
            Page = request.Page,
            PageSize = request.PageSize
        };
        
        int count = await _shipmentRepository.GetShipmentsCount(filter);
        var shipments = await _shipmentRepository.GetShipments(filter, withPagination: true);

        GetShipmentsQueryResponse response = new()
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPage = (int)Math.Ceiling((double) count / request.PageSize),
            Shipments = []
        };

        foreach (var shipment in shipments)
        {
            var shipmentResult = new ShipmentDto
            {
                ShipmentId = shipment.Id.ToString(),
                WarehouseId = shipment.WarehouseId.ToString(),
                StoreId = shipment.StoreId.ToString(),
                ExpectedSendingDateDate = shipment.ExpectedSendingDate,
                SendingDate = shipment.SendingDate,
                SendingById = shipment.SendById.ToString(),
                ShipmentItems = [],
                CreatedAt = shipment.CreatedAt,
                CreatedById = shipment.CreatedById.ToString(),
                CreatedByName = shipment.CreatedByName,
                CreatedByUserName = shipment.CreatedByUserName,
                UpdatedAt = shipment.UpdatedAt,
                UpdatedById = shipment.UpdatedById.ToString(),
                UpdatedByName = shipment.UpdatedByName,
                UpdatedByUserName = shipment.UpdatedByUserName
            };

            response.Shipments.Add(shipmentResult);

            if (shipment.ShipmentItems is not null)
                foreach (var item in shipment.ShipmentItems)
                    shipmentResult.ShipmentItems.Add(new ShipmentItemDto
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

        }

        return response;
        
    }
}