namespace WHMS.Persistence.SeedData.Dto;

public class WasteRecordSeedDto
{
    public List<WasteRecord> WasteRecords { get; set; } = null!;
}

public class WasteRecord
{
    public string Warehouse { get; set; } = null!;
    public string Sku { get; set; } = null!;
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
}