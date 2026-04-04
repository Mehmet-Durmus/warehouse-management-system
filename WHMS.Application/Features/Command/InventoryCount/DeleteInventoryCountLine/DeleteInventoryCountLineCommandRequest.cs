using MediatR;

namespace WHMS.Application.Features.Command.InventoryCount.DeleteInventoryCountLine;

public class DeleteInventoryCountLineCommandRequest : IRequest
{
    public string? InventoryCountLineId { get; set; }
}