namespace WHMS.Application.Features.Queries.WasteRecord.GetWasteRecords;

public class GetWasteRecordsResultWasteRecordDto
{
    public string WasteRecordId { get; set; } = null!;
    public string WarehouseId { get; set; } = null!;
    public string SkuId { get; set; } = null!;
    public string SkuName { get; set; } = null!;
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedById { get; set; } = null!;
    public string CreatedByName { get; set; } = null!;
    public string CreatedByUsername { get; set; } = null!;
}