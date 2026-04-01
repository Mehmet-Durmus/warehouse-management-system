namespace WHMS.Application.Features.Command.WasteRecord.UpdateWasteRecord;

public class UpdateWasteRecordCommandResponse
{
    public string SkuId { get; set; } = null!;
    public string SkuName { get; set; } = null!;
    public int Quantity { get; set; }
    public string? Description { get; set; }
}