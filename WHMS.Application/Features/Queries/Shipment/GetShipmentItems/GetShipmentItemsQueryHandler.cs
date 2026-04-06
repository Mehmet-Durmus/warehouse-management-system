using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Filters;

namespace WHMS.Application.Features.Queries.Shipment.GetShipmentItems;

public class GetShipmentItemsQueryHandler : IRequestHandler<GetShipmentItemsQueryRequest, GetShipmentItemsQueryResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetShipmentItemsQueryHandler(IShipmentRepository shipmentRepository, ICurrentUserService currentUserService)
    {
        _shipmentRepository = shipmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetShipmentItemsQueryResponse> Handle(GetShipmentItemsQueryRequest request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetShipment(Guid.Parse(request.ShipmentId!));
        bool notFound = shipment switch
        {
            null => true,
            not null when _currentUserService.Roles!.Contains(ApplicationRole.WarehouseManager)
                && shipment.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains(ApplicationRole.WarehouseStaff)
                && shipment.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains(ApplicationRole.WarehouseStaff)
                && shipment.WarehouseId == Guid.Parse(_currentUserService.WarehouseId!)
                && shipment.SendingDate != null => true,
            _ => false
        };

        if (notFound)
            throw new Exception("Shipment not found.");

        ShipmentItemFilter filter = new()
        {
            ShipmentId = request.ShipmentId,
            SkuId = request.SkuId,
            MaxQuantity = request.MaxQuantity,
            MinQuantity = request.MinQuantity,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var shipmentItems = await _shipmentRepository.GetShipmentItems(filter, applyPagination: true);
        int count = await _shipmentRepository.GetShipmentItemsCount(filter);

        GetShipmentItemsQueryResponse response = new() { ShipmentItems = [] };
        response.Pagination = new(
            request.Page,
            request.PageSize,
            (int)Math.Ceiling((double) count / request.PageSize)
        );

        foreach (var shipmentItem in shipmentItems)
            response.ShipmentItems.Add(new()
            {
                ShipmentItemId = shipmentItem.Id.ToString(),
                Sku = new()
                {
                    Id = shipmentItem.SKU!.Id.ToString(),
                    SKUName = shipmentItem.SKU!.SKUName,
                    Barcode = shipmentItem.SKU!.Barcode,
                    UnitPrice = shipmentItem.SKU!.UnitPrice
                },
                Quantity = shipmentItem.Quantity
            });

        return response;
    }
}