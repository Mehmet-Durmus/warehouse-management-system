using MediatR;

namespace WHMS.Application.Features.Command.Catalog.CreateSku;

public class CreateSkuCommandRequest : IRequest<CreateSkuCommandResponse>
{
    public string? SkuName { get; set; }
    public string? Barcode { get; set; }
    public decimal UnitPrice { get; set; }
    public string? CategoryId { get; set; }
}