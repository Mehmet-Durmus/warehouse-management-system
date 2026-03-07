using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Catalog.DeleteSku;

public class DeleteSkuCommandHandler : IRequestHandler<DeleteSkuCommandRequest, DeleteSkuCommandResponse>
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSkuCommandHandler(ICatalogRepository catalogRepository, IUnitOfWork unitOfWork)
    {
        _catalogRepository = catalogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteSkuCommandResponse> Handle(DeleteSkuCommandRequest request, CancellationToken cancellationToken)
    {
        await _catalogRepository.DeleteSku(Guid.Parse(request.SkuId!));
        await _unitOfWork.CommitAsync();
        return new();
    }
}