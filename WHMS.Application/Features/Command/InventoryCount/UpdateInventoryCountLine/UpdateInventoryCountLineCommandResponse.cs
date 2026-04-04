namespace WHMS.Application.Features.Command.InventoryCount.UpdateInventoryCountLine;

public class UpdateInventoryCountLineCommandResponse
{
    public string InventoryCountId { get; set; } = null!;
    public string SkuId { get; set; } = null!;
    public int Quantity { get; set; }
    public int Variance { get; set; }
}