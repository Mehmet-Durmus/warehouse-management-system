using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Extensions;
using WHMS.Domain.BusinessRules.Delivery;
using WHMS.Domain.BusinessRules.Warehouse;

namespace WHMS.Application.Features.Command.Delivery.UpdateDelivery;

public class UpdateDeliveryCommandHandler : IRequestHandler<UpdateDeliveryCommandRequest, UpdateDeliveryCommandResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWarehouseRepository _warehouseRepository;

    public UpdateDeliveryCommandHandler(IDeliveryRepository deliveryRepository, IUnitOfWork unitOfWork, IWarehouseRepository warehouseRepository)
    {
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
        _warehouseRepository = warehouseRepository;
    }

    public async Task<UpdateDeliveryCommandResponse> Handle(UpdateDeliveryCommandRequest request, CancellationToken cancellationToken)
    {
        var delivery = DeliveryRules.EnsureExists(await _deliveryRepository.GetDelivery(Guid.Parse(request.DeliveryId!)));
        DeliveryRules.EnsureNotReceived(delivery.ReceivedAt is not null);

        var warehouse = WarehouseRules.EnsureExists(await _warehouseRepository.GetWarehouse(Guid.Parse(request.WarehouseId!)));

        delivery.WarehouseId = Guid.Parse(request.WarehouseId!);
        delivery.ExpectedArrivalDate = request.ExpectedArrivalDate;
        await _unitOfWork.CommitAsync();
        return new()
        {
            WarehouseId = delivery.WarehouseId.ToString(),
            WarehouseName = warehouse.WarehouseName,
            ExpectedArrivalDate = delivery.ExpectedArrivalDate
        };
    }
}