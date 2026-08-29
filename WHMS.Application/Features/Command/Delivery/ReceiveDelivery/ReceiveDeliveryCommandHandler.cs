using System.Runtime.CompilerServices;
using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.Delivery;

namespace WHMS.Application.Features.Command.Delivery.ReceiveDelivery;

public class ReceiveDeliveryCommandHandler : IRequestHandler<ReceiveDeliveryCommandRequest, ReceiveDeliveryCommandResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IStockStateRepository _stockStateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ReceiveDeliveryCommandHandler(IDeliveryRepository deliveryRepository, IStockStateRepository stockStateRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _deliveryRepository = deliveryRepository;
        _stockStateRepository = stockStateRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ReceiveDeliveryCommandResponse> Handle(ReceiveDeliveryCommandRequest request, CancellationToken cancellationToken)
    {
        var warehouseId = Guid.Parse(_currentUserService.WarehouseId!);
        var delivery = DeliveryRules.EnsureAccessibleForReceipt(await _deliveryRepository.GetDelivery(Guid.Parse(request.DeliveryId!)), warehouseId);
        DeliveryRules.EnsureNotAlreadyReceived(delivery.ReceivedAt is not null);
        delivery.ReceivedAt = DateTime.Now;
        delivery.ReceivedById = _currentUserService.UserId;

        if (delivery.DeliveryItems is not null)
            foreach (var item in delivery.DeliveryItems)
                await _stockStateRepository.UpdateQuantity(delivery.WarehouseId, item.SkuId, item.Quantity);

        await _unitOfWork.CommitAsync();

        return new();
    }
}