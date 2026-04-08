namespace WHMS.Persistence.SeedData.Dto;

public class InventoryCountsSeedDto
{
    public string CreatedBy { get; set; } = null!;
    public List<Count> Counts { get; set; } = null!;
}

public class Count
{
    public string Warehouse { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CountLine> CountLines { get; set; } = null!;
}

public class CountLine
{
    public string Sku { get; set; } = null!;
    public int Quantity { get; set; }
    public int Variance { get; set; }
    public string CreatedBy { get; set; } = null!;
}