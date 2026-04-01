namespace WHMS.Application.Features.Command.WasteRecord.CreateWasteRecord;

public class CreateWasteRecordCommandResponse
{
    public string WasteRecordId { get; set; } = null!;
    public string SkuId { get; set; } = null!;
    public string SkuName { get; set; } = null!;
    public int Quantity { get; set; }
    public string? Description { get; set; }
}