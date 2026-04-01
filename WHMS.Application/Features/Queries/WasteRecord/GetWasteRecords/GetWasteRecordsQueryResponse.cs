using WHMS.Application.Common.DTOs;
using WHMS.Application.DTOs.WasteRecord;

namespace WHMS.Application.Features.Queries.WasteRecord.GetWasteRecords;

public class GetWasteRecordsQueryResponse
{
    public PaginationDto Pagination { get; set; } = null!;
    public List<GetWasteRecordsResultWasteRecordDto> WasteRecords { get; set; } = null!;
}