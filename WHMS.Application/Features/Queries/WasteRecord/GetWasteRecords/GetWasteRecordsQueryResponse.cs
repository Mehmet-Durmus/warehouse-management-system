using WHMS.Application.DTOs.WasteRecord;

namespace WHMS.Application.Features.Queries.WasteRecord.GetWasteRecords;

public class GetWasteRecordsQueryResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPage { get; set; }
    public List<WasteRecordDto>? WasteRecords { get; set; }
}