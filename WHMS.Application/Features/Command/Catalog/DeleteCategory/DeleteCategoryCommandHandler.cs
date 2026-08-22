using MediatR;
using WHMS.Application.Abstractions.Persistence;

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
        var category = await _catalogRepository.GetCategory(Guid.Parse(request.CategoryId!));
        if (category is null)
            throw new Exception("Category not found.");

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
        if (inStock || inDelivery)
            throw new Exception("This category cannot be deleted because it contains SKUs that exist in stock or pending deliveries.");

        await _catalogRepository.DeleteCategory(Guid.Parse(request.CategoryId!));
        await _catalogRepository.DeleteSkusByCategory(Guid.Parse(request.CategoryId!));
        await _unitOfWork.CommitAsync();
        return new();
    }
}