using System.Globalization;
using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.Catalog;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Catalog.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommandRequest, UpdateCategoryCommandResponse>
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateCategoryCommandHandler(ICatalogRepository catalogRepository, IUnitOfWork unitOfWork)
    {
        _catalogRepository = catalogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCategoryCommandResponse> Handle(UpdateCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        var category = CategoryRules.EnsureExists(await _catalogRepository.GetCategory(Guid.Parse(request.CategoryId!)));

        var normalizedCategoryName = request.CategoryName!.ToUpper(CultureInfo.GetCultureInfo("tr-TR"));
        if (!category.NormalizedCategoryName.Equals(normalizedCategoryName))
        {
            bool isCategoryNameExists = await _catalogRepository.IsCategoryNameExists(normalizedCategoryName);
            CategoryRules.EnsureNameIsUnique(isCategoryNameExists);
        }
        
        category.CategoryName = request.CategoryName;
        category.NormalizedCategoryName = normalizedCategoryName;

        await _unitOfWork.CommitAsync();

        return new()
        {
            CategoryName = category.CategoryName
        };
    }
}