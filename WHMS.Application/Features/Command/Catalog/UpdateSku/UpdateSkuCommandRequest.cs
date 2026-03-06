using MediatR;

namespace WHMS.Application.Features.Command.Catalog.UpdateSku;

public class UpdateSkuCommandRequest : IRequest<UpdateSkuCommandResponse>
{
    public string? SkuId { get; set; }
    public string? SkuName { get; set; }
    public string? Barcode { get; set; }
    public decimal UnitPrice { get; set; }
    public string? CategoryId { get; set; }
}