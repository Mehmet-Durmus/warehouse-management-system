using MediatR;

namespace WHMS.Application.Features.Command.Catalog.DeleteSku;

public class DeleteSkuCommandRequest : IRequest<DeleteSkuCommandResponse>
{
    public string? SkuId { get; set; }
}