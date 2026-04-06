using System.Runtime.Serialization;
using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.DTOs.Catalog;
using WHMS.Application.DTOs.Shipment;

namespace WHMS.Application.Features.Queries.Shipment.GetShipment;

public class GetShipmentQueryHandler : IRequestHandler<GetShipmentQueryRequest, GetShipmentQueryResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetShipmentQueryHandler(IShipmentRepository shipmentRepository, ICurrentUserService currentUserService)
    {
        _shipmentRepository = shipmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetShipmentQueryResponse> Handle(GetShipmentQueryRequest request, CancellationToken cancellationToken)
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

        return new()
        {
            WarehouseId = shipment!.WarehouseId.ToString(),
            WarehouseName = shipment!.Warehouse!.WarehouseName,
            StoreId = shipment!.StoreId.ToString(),
            StoreName = shipment!.Store!.StoreName,
            ExpectedSendingDate = shipment!.ExpectedSendingDate,
            SendingDate = shipment!.SendingDate,
            SentById = shipment!.SentById.ToString()
        };
    }
}