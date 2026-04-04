using MediatR;

namespace WHMS.Application.Features.Command.InventoryCount.DeleteInventoryCount;

public class DeleteInventoryCountCommandRequest : IRequest
{
    public string? InventoryCountId { get; set; }
}