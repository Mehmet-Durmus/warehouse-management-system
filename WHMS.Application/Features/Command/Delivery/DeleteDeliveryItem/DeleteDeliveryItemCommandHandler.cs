using System.Runtime.CompilerServices;
using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Delivery.DeleteDeliveryItem;

public class DeleteDeliveryItemCommandHandler : IRequestHandler<DeleteDeliveryItemCommandRequest, DeleteDeliveryItemCommandResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteDeliveryItemCommandHandler(IDeliveryRepository deliveryRepository, IUnitOfWork unitOfWork)
    {
        _deliveryRepository = deliveryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteDeliveryItemCommandResponse> Handle(DeleteDeliveryItemCommandRequest request, CancellationToken cancellationToken)
    {
        await _deliveryRepository.DeleteDeliveryItem(Guid.Parse(request.DeliveryItemId!));
        await _unitOfWork.CommitAsync();
        return new();
    }
}