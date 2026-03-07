using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Store.DeleteStore;

public class DeleteStoreCommandHandler : IRequestHandler<DeleteStoreCommandRequest, DeleteStoreCommandResponse>
{
    private readonly IStoreRepository _storeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteStoreCommandHandler(IStoreRepository storeRepository, IUnitOfWork unitOfWork)
    {
        _storeRepository = storeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteStoreCommandResponse> Handle(DeleteStoreCommandRequest request, CancellationToken cancellationToken)
    {
        await _storeRepository.SoftDelete(Guid.Parse(request.StoreId!));
        await _unitOfWork.CommitAsync();
        return new();
    }
}