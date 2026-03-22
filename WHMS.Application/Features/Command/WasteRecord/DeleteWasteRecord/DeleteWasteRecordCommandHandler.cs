using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.WasteRecord.DeleteWasteRecord;

public class DeleteWasteRecordCommandHandler : IRequestHandler<DeleteWasteRecordCommandRequest, DeleteWasteRecordCommandResponse>
{
    private readonly IWasteRecordRepository _wasteRecordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWasteRecordCommandHandler(IWasteRecordRepository wasteRecordRepository, IUnitOfWork unitOfWork)
    {
        _wasteRecordRepository = wasteRecordRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteWasteRecordCommandResponse> Handle(DeleteWasteRecordCommandRequest request, CancellationToken cancellationToken)
    {
        await _wasteRecordRepository.Delete(Guid.Parse(request.WasteRecordId!));
        await _unitOfWork.CommitAsync();
        return new();
    }
}