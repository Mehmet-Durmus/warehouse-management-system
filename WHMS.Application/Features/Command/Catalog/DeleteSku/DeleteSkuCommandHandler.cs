using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.Catalog;

namespace WHMS.Application.Features.Command.Catalog.DeleteSku;

public class DeleteSkuCommandHandler : IRequestHandler<DeleteSkuCommandRequest, DeleteSkuCommandResponse>
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStockStateRepository _stockStateRepository;
    private readonly IDeliveryRepository _deliveryRepository;

    public DeleteSkuCommandHandler(ICatalogRepository catalogRepository, IUnitOfWork unitOfWork, IStockStateRepository stockStateRepository, IDeliveryRepository deliveryRepository)
    {
        _catalogRepository = catalogRepository;
        _unitOfWork = unitOfWork;
        _stockStateRepository = stockStateRepository;
        _deliveryRepository = deliveryRepository;
    }

    public async Task<DeleteSkuCommandResponse> Handle(DeleteSkuCommandRequest request, CancellationToken cancellationToken)
    {
        bool hasStockInAnyWarehouse = await _stockStateRepository.HasStockInAnyWarehouse(Guid.Parse(request.SkuId!));
        bool hasIncomingDeliveriesWithSku = await _deliveryRepository.HasIncomingDeliveriesWithSku(Guid.Parse(request.SkuId!));
        SkuRules.EnsureCanBeDeleted(hasStockInAnyWarehouse || hasIncomingDeliveriesWithSku);
        
        await _catalogRepository.DeleteSku(Guid.Parse(request.SkuId!));
        await _unitOfWork.CommitAsync();
        return new();
    }
}