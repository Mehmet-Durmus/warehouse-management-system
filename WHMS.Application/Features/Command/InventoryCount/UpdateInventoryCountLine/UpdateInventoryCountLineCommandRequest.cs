using MediatR;

namespace WHMS.Application.Features.Command.InventoryCount.UpdateInventoryCountLine;

public class UpdateInventoryCountLineCommandRequest : IRequest<UpdateInventoryCountLineCommandResponse>
{
    public string? InventoryCountLineId { get; set; }
    public string? InventoryCountId { get; set; }
    public string? SkuId { get; set; }
    public int Quantity { get; set; }
}