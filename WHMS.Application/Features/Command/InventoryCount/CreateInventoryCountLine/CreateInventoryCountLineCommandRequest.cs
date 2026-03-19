using MediatR;

namespace WHMS.Application.Features.Command.InventoryCount.CreateInventoryCountLine;

public class CreateInventoryCountLineCommandRequest : IRequest<CreateInventoryCountLineCommandResponse>
{
    public string? InventoryCountId { get; set; }
    public string? SkuId { get; set; }
    public int Quantity { get; set; }
}