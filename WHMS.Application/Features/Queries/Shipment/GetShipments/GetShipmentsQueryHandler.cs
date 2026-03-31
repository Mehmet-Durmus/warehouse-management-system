using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;

namespace WHMS.Application.Features.Queries.Shipment.GetShipments;

public class GetShipmentsQueryHandler : IRequestHandler<GetShipmentsQueryRequest, GetShipmentsQueryResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetShipmentsQueryHandler(IShipmentRepository shipmentRepository, ICurrentUserService currentUserService)
    {
        _shipmentRepository = shipmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetShipmentsQueryResponse> Handle(GetShipmentsQueryRequest request, CancellationToken cancellationToken)
    {
        var roles = _currentUserService.Roles;
        ShipmentFilter filter = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };
        if (roles!.Contains("LogisticDirector"))
        {
            filter.WarehouseId = request.WarehouseId;
            filter.WarehouseCityId = request.WarehouseCityId;
            filter.WarehouseDistrictId = request.WarehouseDistrictId;
            filter.WarehouseNeighborhoodId = request.WarehouseNeighborhoodId;
            filter.StoreId = request.StoreId;
            filter.StoreCityId = request.StoreCityId;
            filter.StoreDistrictId = request.StoreDistrictId;
            filter.StoreNeighborhoodId = request.StoreNeighborhoodId;
            filter.SkuIds = request.SkuIds;
            filter.IsSent = request.IsSent;
            filter.SentAfter = request.SentAfter;
            filter.SentBefore = request.SentBefore;
            filter.ExpectedSendingDateAfter = request.ExpectedSendingDateAfter;
            filter.ExpectedSendingDateBefore = request.ExpectedSendingDateBefore;
            filter.CreatedAfter = request.CreatedAfter;
            filter.CreatedBefore = request.CreatedBefore;
            filter.UpdatedAfter = request.UpdatedAfter;
            filter.UpdatedBefore = request.UpdatedBefore;
        }
        else if (roles!.Contains("WarehouseManager"))
        {
            filter.WarehouseId = _currentUserService.WarehouseId;
            filter.SkuIds = request.SkuIds;
            filter.IsSent = request.IsSent;
            filter.SentAfter = request.SentAfter;
            filter.SentBefore = request.SentBefore;
            filter.ExpectedSendingDateAfter = request.ExpectedSendingDateAfter;
            filter.ExpectedSendingDateBefore = request.ExpectedSendingDateBefore;
        }
        else if (roles!.Contains("WarehouseStaff"))
        {
            filter.WarehouseId = _currentUserService.WarehouseId;
            filter.IsSent = false;
        }

        var shipments = await _shipmentRepository.GetShipments(filter, applyPagination: true);
        int count = await _shipmentRepository.GetShipmentsCount(filter);

        GetShipmentsQueryResponse response = new() { Shipments = [] };
        response.Pagination = new(
            request.Page,
            request.PageSize,
            (int)Math.Ceiling((double) count / request.PageSize)
        );

        foreach (var shipment in shipments)
            response.Shipments.Add(new()
            {
                ShipmentId = shipment.Id.ToString(),
                WarehouseId = shipment.WarehouseId.ToString(),
                WarehouseName = shipment.Warehouse!.WarehouseName,
                StoreId = shipment.StoreId.ToString(),
                StoreName = shipment.Store!.StoreName,
                ExpectedSendingDate = shipment.ExpectedSendingDate,
                SendingDate = shipment.SendingDate,
                SentById = shipment.SentById.ToString()
            });

        return response;
    }
}