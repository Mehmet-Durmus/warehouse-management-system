namespace WHMS.Application.Features.Queries.Catalog.GetCatalogData;

public class GetCatalogDataResultSkuDto
{
    public string Id { get; set; } = null!;
    public string SKUName { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public decimal UnitPrice { get; set; }
}