namespace WHMS.Application.Features.Queries.WasteRecord.GetWasteRecord;

public class GetWasteRecordQueryResponse
{
    public string WarehouseId { get; set; } = null!;
    public string SkuId { get; set; } = null!;
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedById { get; set; } = null!;
}