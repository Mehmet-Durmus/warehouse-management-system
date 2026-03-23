using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.WasteRecord.CreateWasteRecord;

public class CreateWasteRecordCommandHandler : IRequestHandler<CreateWasteRecordCommandRequest, CreateWasteRecordCommandResponse>
{
    private readonly IWasteRecordRepository _wasteRecordRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStockStateRepository _stockStateRepository;

    public CreateWasteRecordCommandHandler(IWasteRecordRepository wasteRecordRepository, IUnitOfWork unitOfWork, IStockStateRepository stockStateRepository)
    {
        _wasteRecordRepository = wasteRecordRepository;
        _unitOfWork = unitOfWork;
        _stockStateRepository = stockStateRepository;
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
        var affectedRow = await _stockStateRepository.SetQuantity(wasteRecord.WarehouseId, wasteRecord.SkuId, -wasteRecord.Quantity);
        if (affectedRow == 0)
            throw new Exception("Insufficient stock exception.");
        await _unitOfWork.CommitAsync();

        return new();
    }
}