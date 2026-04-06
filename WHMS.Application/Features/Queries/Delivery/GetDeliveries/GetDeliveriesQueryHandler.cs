using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Filters;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveries;

public class GetDeliveriesQueryHandler : IRequestHandler<GetDeliveriesQueryRequest, GetDeliveriesQueryResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetDeliveriesQueryHandler(IDeliveryRepository deliveryRepository, ICurrentUserService currentUserService)
    {
        _deliveryRepository = deliveryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetDeliveriesQueryResponse> Handle(GetDeliveriesQueryRequest request, CancellationToken cancellationToken)
    {
        var userRoles = _currentUserService.Roles;
        DeliveryFilter filter = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };
        if (userRoles!.Contains(ApplicationRole.LogisticDirector))
        {
            filter.WarehouseId = request.WarehouseId;
            filter.CityId = request.CityId;
            filter.DistrictId = request.DistrictId;
            filter.SkuIds = request.SkuIds;
            filter.IsReceived = request.IsReceived;
            filter.ReceivedAfter = request.ReceivedAfter;
            filter.ReceivedBefore = request.ReceivedBefore;
            filter.ExpectedArrivalAfter = request.ExpectedArrivalAfter;
            filter.ExpectedArrivalBefore = request.ExpectedArrivalBefore;
            filter.CreatedAfter = request.CreatedAfter;
            filter.CreatedBefore = request.CreatedBefore;
            filter.UpdatedAfter = request.UpdatedAfter;
            filter.UpdatedBefore = request.UpdatedBefore;
        }
        else if (userRoles.Contains(ApplicationRole.WarehouseManager))
        {
            filter.WarehouseId = _currentUserService.WarehouseId;
            filter.IsReceived = request.IsReceived;
            filter.SkuIds = request.SkuIds;
            filter.ReceivedAfter = request.ReceivedAfter;
            filter.ReceivedBefore = request.ReceivedBefore;
            filter.ExpectedArrivalAfter = request.ExpectedArrivalAfter;
            filter.ExpectedArrivalBefore = request.ExpectedArrivalBefore;
        }
        else if (userRoles.Contains(ApplicationRole.WarehouseStaff))
        {
            filter.WarehouseId = _currentUserService.WarehouseId;
            filter.IsReceived = false;
        }
        else
            throw new Exception("Internal - Unauthorized");
    
        var deliveries = await _deliveryRepository.GetDeliveries(filter, true);
        int count = await _deliveryRepository.GetDeliveriesCount(filter);
        GetDeliveriesQueryResponse response = new() { Deliveries = [] };
        response.Pagination = new(
            request.Page,
            request.PageSize,
            (int)Math.Ceiling((double) count / request.PageSize)
        );

        foreach (var delivery in deliveries)
            response.Deliveries.Add(new()
            {
                DeliveryId = delivery.Id.ToString(),
                WarehouseId = delivery.WarehouseId.ToString(),
                WarehouseName = delivery.Warehouse!.WarehouseName,
                ExpectedArrivalDate = delivery.ExpectedArrivalDate,
                IsReceived = delivery.ReceivedAt != null,
                ReceivedAt = delivery.ReceivedAt,
                ReceivedById = delivery.ReceivedById.ToString()
            });
        return response;
    }
}