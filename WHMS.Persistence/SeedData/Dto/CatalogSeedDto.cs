namespace WHMS.Persistence.SeedData.Dto;

public class CatalogSeedDto
{
    public List<Categories> Categories { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
}

public class Categories
{
    public string CategoryName { get; set; } = null!;
    public List<Skus> Skus { get; set; } = null!;
}

public class Skus
{
    public string SKUName { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public decimal UnitPrice { get; set; }
}