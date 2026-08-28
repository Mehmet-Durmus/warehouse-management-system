using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.Catalog;

namespace WHMS.Application.Features.Command.Catalog.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommandRequest, DeleteCategoryCommandResponse>
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStockStateRepository _stockStateRepository;
    private readonly IDeliveryRepository _deliveryRepository;
    public DeleteCategoryCommandHandler(ICatalogRepository catalogRepository, IUnitOfWork unitOfWork, IStockStateRepository stockStateRepository, IDeliveryRepository deliveryRepository)
    {
        _catalogRepository = catalogRepository;
        _unitOfWork = unitOfWork;
        _stockStateRepository = stockStateRepository;
        _deliveryRepository = deliveryRepository;
    }

    public async Task<DeleteCategoryCommandResponse> Handle(DeleteCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        var category = CategoryRules.EnsureExists(await _catalogRepository.GetCategory(Guid.Parse(request.CategoryId!)));

        bool inStock = false;
        bool inDelivery = false;
        if (category.Skus is not null)
            foreach (var sku in category.Skus)
            {
                inStock = await _stockStateRepository.HasStockInAnyWarehouse(sku.Id);
                inDelivery = await _deliveryRepository.HasIncomingDeliveriesWithSku(sku.Id);
                if (inStock || inDelivery)
                    break;
            }
        CategoryRules.EnsureCanBeDeleted(inStock || inDelivery);

        await _catalogRepository.DeleteCategory(Guid.Parse(request.CategoryId!));
        await _catalogRepository.DeleteSkusByCategory(Guid.Parse(request.CategoryId!));
        await _unitOfWork.CommitAsync();
        return new();
    }
}