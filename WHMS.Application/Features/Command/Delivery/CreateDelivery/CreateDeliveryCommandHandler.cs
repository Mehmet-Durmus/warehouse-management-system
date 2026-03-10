using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Delivery.CreateDelivery;

public class CreateDeliveryCommandHandler : IRequestHandler<CreateDeliveryCommandRequest, CreateDeliveryCommandResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDeliveryCommandHandler(IDeliveryRepository deliveryRepository, IUnitOfWork unitOfWork)
    {
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateDeliveryCommandResponse> Handle(CreateDeliveryCommandRequest request, CancellationToken cancellationToken)
    {
        var delivery = new Domain.Entities.Delivery
        {
            WarehouseId = Guid.Parse(request.WarehouseId!)
        };
        await _deliveryRepository.CreateDelivery(delivery);
        await _unitOfWork.CommitAsync();
        return new()
        {
            Id = delivery.Id.ToString(),
            WarehouseId = delivery.WarehouseId.ToString(),
            CreatedByUserName = delivery.CreatedByUserName
        };
    }
}