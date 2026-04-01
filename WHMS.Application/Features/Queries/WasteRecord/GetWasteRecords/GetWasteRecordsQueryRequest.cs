using MediatR;

namespace WHMS.Application.Features.Queries.WasteRecord.GetWasteRecords;

public class GetWasteRecordsQueryRequest : IRequest<GetWasteRecordsQueryResponse>
{
    public string? WarehouseId { get; set; }
    public string? CreatedById { get; set; }
    public string? SkuId { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}