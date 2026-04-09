using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;

namespace WHMS.Application.Features.Queries.Delivery.GetDelivery;

public class GetDeliveryQueryHandler : IRequestHandler<GetDeliveryQueryRequet, GetDeliveryQueryResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetDeliveryQueryHandler(IDeliveryRepository deliveryRepository, ICurrentUserService currentUserService)
    {
        _deliveryRepository = deliveryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetDeliveryQueryResponse> Handle(GetDeliveryQueryRequet request, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryRepository.GetDelivery(Guid.Parse(request.DeliveryId!));
        bool notFound = delivery switch
        {
            null => true,
            not null when _currentUserService.Roles!.Contains(ApplicationRole.WarehouseManager)
                && delivery.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains(ApplicationRole.WarehouseStaff)
                && delivery.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains(ApplicationRole.WarehouseStaff)
                && delivery.WarehouseId == Guid.Parse(_currentUserService.WarehouseId!)
                && delivery.ReceivedAt != null => true,
            _ => false
        };

        if (notFound)
            throw new Exception("Delivery not found.");
        
        return new()
        {
            WarehouseId = delivery!.WarehouseId.ToString(),
            WarehouseName = delivery.Warehouse!.WarehouseName,
            ExpectedArrivalDate = delivery.ExpectedArrivalDate,
            IsReceived = delivery.ReceivedAt != null,
            ReceivedAt = delivery.ReceivedAt,
            ReceivedById = delivery.ReceivedById.ToString()
        };
    }
}