using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.Warehouse;

namespace WHMS.Application.Features.Command.Delivery.CreateDelivery;

public class CreateDeliveryCommandHandler : IRequestHandler<CreateDeliveryCommandRequest, CreateDeliveryCommandResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWarehouseRepository _warehouseRepository;

    public CreateDeliveryCommandHandler(IDeliveryRepository deliveryRepository, IUnitOfWork unitOfWork, IWarehouseRepository warehouseRepository)
    {
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
        _warehouseRepository = warehouseRepository;
    }

    public async Task<CreateDeliveryCommandResponse> Handle(CreateDeliveryCommandRequest request, CancellationToken cancellationToken)
    {
        var warehouse = WarehouseRules.EnsureExists(await _warehouseRepository.GetWarehouse(Guid.Parse(request.WarehouseId!)));

        var delivery = new Domain.Entities.Delivery
        {
            WarehouseId = Guid.Parse(request.WarehouseId!),
            ExpectedArrivalDate = request.ExpectedArrivalDate
        };
        await _deliveryRepository.CreateDelivery(delivery);
        await _unitOfWork.CommitAsync();
        return new()
        {
            DeliveryId = delivery.Id.ToString(),
            WarehouseId = delivery.WarehouseId.ToString(),
            WarehouseName = warehouse.WarehouseName,
            ExpectedArrivalDate = delivery.ExpectedArrivalDate
        };
    }
}