using MediatR;

namespace WHMS.Application.Features.Command.InventoryCount.DeleteInventoryCountLine;

public class DeleteInventoryCountLineCommandRequest : IRequest<DeleteInventoryCountLineCommandResponse>
{
    public string? InventoryCountLineId { get; set; }
}