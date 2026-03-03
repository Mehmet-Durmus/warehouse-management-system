using System.Globalization;
using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Catalog.CreateSku;

public class CreateSkuCommandHandler : IRequestHandler<CreateSkuCommandRequest, CreateSkuCommandResponse>
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSkuCommandHandler(ICatalogRepository catalogRepository, IUnitOfWork unitOfWork)
    {
        _catalogRepository = catalogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateSkuCommandResponse> Handle(CreateSkuCommandRequest request, CancellationToken cancellationToken)
    {
        string normalizedSkuName = request.SkuName!.ToUpper(CultureInfo.GetCultureInfo("tr-TR"));
        bool isSkuNameExists = await _catalogRepository.IsSkuNameExists(normalizedSkuName);
        if(isSkuNameExists)
            throw new Exception("Sku already exists.");
        
        var sku = new SKU
        {
            SKUName = request.SkuName,
            NormalizedSKUName = normalizedSkuName,
            Barcode = request.Barcode!,
            UnitPrice = request.UnitPrice,
            CategoryId = Guid.Parse(request.CategoryId!)
        };

        _catalogRepository.UpdateSku(sku);
        await _unitOfWork.CommitAsync();

        return new()
        {
            SkuId = sku.Id.ToString(),
            CategoryId = sku.CategoryId.ToString(),
            Barcode = sku.Barcode,
            UnitPrice = sku.UnitPrice,
            SkuName = sku.SKUName
        };
    }
}