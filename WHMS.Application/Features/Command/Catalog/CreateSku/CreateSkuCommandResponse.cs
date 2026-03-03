namespace WHMS.Application.Features.Command.Catalog.CreateSku;

public class CreateSkuCommandResponse
{
    public string? SkuId { get; set; }
    public string? CategoryId { get; set; }
    public string? Barcode { get; set; }
    public decimal UnitPrice { get; set; }
    public string? SkuName { get; set; }
}