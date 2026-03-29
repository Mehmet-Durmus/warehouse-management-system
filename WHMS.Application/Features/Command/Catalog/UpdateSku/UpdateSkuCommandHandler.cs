using System.Globalization;
using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Catalog.UpdateSku;

public class UpdateSkuCommandHandler : IRequestHandler<UpdateSkuCommandRequest, UpdateSkuCommandResponse>
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSkuCommandHandler(ICatalogRepository catalogRepository, IUnitOfWork unitOfWork)
    {
        _catalogRepository = catalogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateSkuCommandResponse> Handle(UpdateSkuCommandRequest request, CancellationToken cancellationToken)
    {
        var sku = await _catalogRepository.GetSku(Guid.Parse(request.SkuId!));
        if (sku is null)
            throw new Exception("Sku not found.");

        var normalizedSkuName = request.SkuName!.ToUpper(CultureInfo.GetCultureInfo("tr-TR"));
        if (!sku.NormalizedSKUName.Equals(normalizedSkuName))
        {
            bool isSkuNameExists = await _catalogRepository.IsSkuNameExists(normalizedSkuName);
            if (isSkuNameExists)
                throw new Exception("Sku already exists.");
        }
        
        var category = await _catalogRepository.GetCategory(Guid.Parse(request.CategoryId!));
        if (category is null)
            throw new Exception("Category not found");
        
        sku.SKUName = request.SkuName;
        sku.NormalizedSKUName = normalizedSkuName;
        sku.Barcode = request.Barcode!;
        sku.CategoryId = category.Id;
        sku.UnitPrice = request.UnitPrice;

        _catalogRepository.UpdateSku(sku);
        await _unitOfWork.CommitAsync();

        return new()
        {
            CategoryId = sku.CategoryId.ToString(),
            Barcode = sku.Barcode,
            UnitPrice = sku.UnitPrice,
            SkuName = sku.SKUName
        };
    }
}