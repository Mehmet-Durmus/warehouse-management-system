namespace WHMS.Application.DTOs.Catalog;

public class SkuDto
{
    public string Id { get; set; } = null!;
    public string SKUName { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public decimal UnitPrice { get; set; }
}