using System.Globalization;
using MediatR;
using WHMS.Application.Abstractions.Persistence;
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
        var normalizedCategoryName = request.CategoryName!.ToUpper(CultureInfo.GetCultureInfo("tr-TR"));
        bool isCategoryNameExists = await _catalogRepository.IsCategoryNameExists(normalizedCategoryName);

        if (isCategoryNameExists)
            throw new Exception("Category already exists.");

        var category = await _catalogRepository.GetCategory(Guid.Parse(request.CategoryId!));
        category.CategoryName = request.CategoryName;
        category.NormalizedCategoryName = normalizedCategoryName;

        _catalogRepository.UpdateCategory(category);
        await _unitOfWork.CommitAsync();

        return new()
        {
            CategoryId = category.Id.ToString(),
            CategoryName = category.CategoryName
        };
    }
}