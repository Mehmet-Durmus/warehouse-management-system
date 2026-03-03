using System.Globalization;
using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Catalog;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommandRequest, CreateCategoryCommandResponse>
{  
    private readonly ICatalogRepository _catalogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(ICatalogRepository catalogRepository, IUnitOfWork unitOfWork)
    {
        _catalogRepository = catalogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCategoryCommandResponse> Handle(CreateCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        var normalizedCategoryName = request.CategoryName!.ToUpper(CultureInfo.GetCultureInfo("tr-TR"));
        var IsCategoryNameExists = await _catalogRepository.IsCategoryNameExists(normalizedCategoryName);
        if (IsCategoryNameExists)
            throw new Exception("Category already exists.");
        
        var newCategory = new Category
        {
           CategoryName = request.CategoryName!,
           NormalizedCategoryName = normalizedCategoryName
        };
        await _catalogRepository.AddCategory(newCategory);
        await _unitOfWork.CommitAsync();

        return new()
        {
            CategoryId = newCategory.Id.ToString(),
            CategoryName = newCategory.CategoryName
        };
    }
}