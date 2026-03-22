using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Filters;

namespace WHMS.Application.Features.Queries.WasteRecord.GetWasteRecords;

public class GetWasteRecordsQueryHandler : IRequestHandler<GetWasteRecordsQueryRequest, GetWasteRecordsQueryResponse>
{
    private readonly IWasteRecordRepository _wasteRecordRepository;

    public GetWasteRecordsQueryHandler(IWasteRecordRepository wasteRecordRepository)
    {
        _wasteRecordRepository = wasteRecordRepository;
    }

    public async Task<GetWasteRecordsQueryResponse> Handle(GetWasteRecordsQueryRequest request, CancellationToken cancellationToken)
    {
        WasteRecordFilter filter = new()
        {
            WarehouseId = request.WarehouseId,
            SkuId = request.SkuId,
            MinQuantity = request.MinQuantity,
            MaxQuantity = request.MaxQuantity,
            Page = request.Page,
            PageSize = request.PageSize
        };

        int count = await _wasteRecordRepository.GetWasteRecordCount(filter);
        var wasteRecords = await _wasteRecordRepository.GetWasteRecords(filter, withPagination: true);

        GetWasteRecordsQueryResponse response = new()
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPage = (int)Math.Ceiling((double)count / request.PageSize),
            WasteRecords = []
        };

        foreach (var wasteRecord in wasteRecords)
            response.WasteRecords.Add(new DTOs.WasteRecord.WasteRecordDto
            {
                WasteRecordId = wasteRecord.Id.ToString(),
                WarehouseId = wasteRecord.WarehouseId.ToString(),
                SkuId = wasteRecord.SkuId.ToString(),
                Quantity = wasteRecord.Quantity,
                Description = wasteRecord.Description,
                CreatedAt = wasteRecord.CreatedAt,
                CreatedById = wasteRecord.CreatedById.ToString()!
            });

        return response;
    }
}