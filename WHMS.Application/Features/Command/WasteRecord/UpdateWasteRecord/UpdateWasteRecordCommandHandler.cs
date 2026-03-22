using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.WasteRecord.UpdateWasteRecord;

public class UpdateWasteRecordCommandHandler : IRequestHandler<UpdateWasteRecordCommandRequest, UpdateWasteRecordCommandResponse>
{
    private readonly IWasteRecordRepository _wasteRecordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWasteRecordCommandHandler(IWasteRecordRepository wasteRecordRepository, IUnitOfWork unitOfWork)
    {
        _wasteRecordRepository = wasteRecordRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateWasteRecordCommandResponse> Handle(UpdateWasteRecordCommandRequest request, CancellationToken cancellationToken)
    {
        var wasteRecord = await _wasteRecordRepository.GetWasteRecord(Guid.Parse(request.WasteRecordId!));
        if (wasteRecord is null)
            throw new Exception("Waste record not found.");
        
        wasteRecord.WarehouseId = Guid.Parse(request.WarehouseId!);
        wasteRecord.SkuId = Guid.Parse(request.SkuId!);
        wasteRecord.Quantity = request.Quantity;
        wasteRecord.Description = request.Description;

        await _unitOfWork.CommitAsync();
        
        return new();
    }
}