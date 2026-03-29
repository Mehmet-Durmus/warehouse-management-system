namespace WHMS.Application.Features.Command.Catalog.UpdateSku;

public class UpdateSkuCommandResponse
{
    public string? CategoryId { get; set; }
    public string? Barcode { get; set; }
    public decimal UnitPrice { get; set; }
    public string? SkuName { get; set; }
}