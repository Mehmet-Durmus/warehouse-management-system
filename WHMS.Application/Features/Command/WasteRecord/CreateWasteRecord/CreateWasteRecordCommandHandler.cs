using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.WasteRecord.CreateWasteRecord;

public class CreateWasteRecordCommandHandler : IRequestHandler<CreateWasteRecordCommandRequest, CreateWasteRecordCommandResponse>
{
    private readonly IWasteRecordRepository _wasteRecordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWasteRecordCommandHandler(IWasteRecordRepository wasteRecordRepository, IUnitOfWork unitOfWork)
    {
        _wasteRecordRepository = wasteRecordRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateWasteRecordCommandResponse> Handle(CreateWasteRecordCommandRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.WasteRecord wasteRecord = new()
        {
            WarehouseId = Guid.Parse(request.WarehouseId!),
            SkuId = Guid.Parse(request.SkuId!),
            Quantity = request.Quantity ?? 0,
            Description = request.Description
        };

        await _wasteRecordRepository.CreateWasteRecord(wasteRecord);
        await _unitOfWork.CommitAsync();

        return new();
    }
}