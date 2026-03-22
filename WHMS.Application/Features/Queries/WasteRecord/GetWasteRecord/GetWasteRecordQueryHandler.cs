using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.WasteRecord.GetWasteRecord;

public class GetWasteRecordQueryHandler : IRequestHandler<GetWasteRecordQueryRequest, GetWasteRecordQueryResponse>
{
    private readonly IWasteRecordRepository _wasteRecordRepository;

    public GetWasteRecordQueryHandler(IWasteRecordRepository wasteRecordRepository)
    {
        _wasteRecordRepository = wasteRecordRepository;
    }

    public async Task<GetWasteRecordQueryResponse> Handle(GetWasteRecordQueryRequest request, CancellationToken cancellationToken)
    {
        var wasteRecord = await _wasteRecordRepository.GetWasteRecord(Guid.Parse(request.WasteRecordId!));
        if (wasteRecord is null)
            throw new Exception("Waste record not found.");

        return new()
        {
            WarehouseId = wasteRecord.WarehouseId.ToString(),
            SkuId = wasteRecord.SkuId.ToString(),
            Quantity = wasteRecord.Quantity,
            Description = wasteRecord.Description,
            CreatedAt = wasteRecord.CreatedAt,
            CreatedById = wasteRecord.CreatedById.ToString()!
        };
    }
}