using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Extensions;

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
        var delivery = await _deliveryRepository.GetDelivery(Guid.Parse(request.DeliveryId!));
        if (delivery is null)
            throw new Exception("Delivery not found.");
        
        if (delivery.ReceivedAt is not null)
            throw new Exception("Delivery has already been received.");
        
        var warehouse = await _warehouseRepository.GetWarehouse(Guid.Parse(request.WarehouseId!))!;
        if (warehouse is null)
            throw new Exception("Warehouse not found.");

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