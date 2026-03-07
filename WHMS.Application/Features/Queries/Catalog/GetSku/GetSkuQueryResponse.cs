namespace WHMS.Application.Features.Queries.Catalog.GetSku;

public class GetSkuQueryResponse
{
    public string? SKUName { get; set; }
    public string? Barcode { get; set; }
    public decimal UnitPrice { get; set; }
}