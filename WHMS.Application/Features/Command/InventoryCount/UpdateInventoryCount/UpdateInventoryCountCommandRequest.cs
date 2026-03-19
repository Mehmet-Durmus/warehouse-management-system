using MediatR;

namespace WHMS.Application.Features.Command.InventoryCount.UpdateInventoryCount;

public class UpdateInventoryCountCommandRequest : IRequest<UpdateInventoryCountCommandResponse>
{
    public string? InventoryCountId { get; set; }
    public string? WarehouseId { get; set; }
}