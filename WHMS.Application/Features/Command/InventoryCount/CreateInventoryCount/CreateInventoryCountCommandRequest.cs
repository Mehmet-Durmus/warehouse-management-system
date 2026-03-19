using MediatR;

namespace WHMS.Application.Features.Command.InventoryCount.CreateInventoryCount;

public class CreateInventoryCountCommandRequest : IRequest<CreateInventoryCountCommandResponse>
{
    public string? WarehouseId { get; set; }
}