namespace WHMS.Application.Features.Command.InventoryCount.CreateInventoryCountLine;

public class CreateInventoryCountLineCommandResponse
{
    public string InventoryCountLineId { get; set; } = null!;
    public string InventoryCountId { get; set; } = null!;
    public string SkuId { get; set; } = null!;
    public int Quantity { get; set; }
    public int Variance { get; set; }
}