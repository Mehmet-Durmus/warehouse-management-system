using MediatR;

namespace WHMS.Application.Features.Command.InventoryCount.DeleteInventoryCount;

public class DeleteInventoryCountCommandRequest : IRequest<DeleteInventoryCountCommandResponse>
{
    public string? InventoryCountId { get; set; }
}