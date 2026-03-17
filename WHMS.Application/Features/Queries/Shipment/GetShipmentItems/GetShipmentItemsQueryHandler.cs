using MediatR;
using Microsoft.AspNetCore.Http.Internal;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.DTOs.Catalog;
using WHMS.Application.DTOs.Shipment;
using WHMS.Application.Filters;

namespace WHMS.Application.Features.Queries.Shipment.GetShipmentItems;

public class GetShipmentItemsQueryHandler : IRequestHandler<GetShipmentItemsQueryRequest, GetShipmentItemsQueryResponse>
{
    private readonly IShipmentRepository _shipmentRepository;

    public GetShipmentItemsQueryHandler(IShipmentRepository shipmentRepository)
    {
        _shipmentRepository = shipmentRepository;
    }

    public async Task<GetShipmentItemsQueryResponse> Handle(GetShipmentItemsQueryRequest request, CancellationToken cancellationToken)
    {
        ShipmentItemFilter filter = new()
        {
            ShipmentId = request.ShipmentId,
            SkuId = request.SkuId,
            MaxQuantity = request.MaxQuantity,
            MinQuantity = request.MinQuantity,
            Page = request.Page,
            PageSize = request.PageSize
        };

        int count = await _shipmentRepository.GetShipmentItemsCount(filter);
        var shipmentItems = await _shipmentRepository.GetShipmentItems(filter, withPagination: true);

        GetShipmentItemsQueryResponse response = new() 
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPage = (int)Math.Ceiling((double)count / request.PageSize),
            ShipmentItems = []
        };
        foreach (var item in shipmentItems)
            response.ShipmentItems.Add(new ShipmentItemDto
            {
                ShipmentItemId = item.Id.ToString(),
                Sku = new SkuDto
                {
                    Id = item.SkuId.ToString(),
                    SKUName = item.SKU?.SKUName,
                    Barcode = item.SKU?.Barcode,
                    UnitPrice = item.SKU!.UnitPrice
                },
                Quantity = item.Quantity
            });

        return response;
    }
}