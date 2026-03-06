using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Catalog.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommandRequest, DeleteCategoryCommandResponse>
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteCategoryCommandHandler(ICatalogRepository catalogRepository, IUnitOfWork unitOfWork)
    {
        _catalogRepository = catalogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteCategoryCommandResponse> Handle(DeleteCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        await _catalogRepository.DeleteCategory(Guid.Parse(request.CategoryId!));
        await _catalogRepository.DeleteSkusByCategory(Guid.Parse(request.CategoryId!));
        await _unitOfWork.CommitAsync();
        return new();
    }
}