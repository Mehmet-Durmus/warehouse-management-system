using MediatR;

namespace WHMS.Application.Features.Command.InventoryCount.CompleteInventoryCount;

public class CompleteInventoryCountCommandRequest : IRequest<CompleteInventoryCountCommandResponse>
{
    public string? InventoryCountId { get; set; }
}