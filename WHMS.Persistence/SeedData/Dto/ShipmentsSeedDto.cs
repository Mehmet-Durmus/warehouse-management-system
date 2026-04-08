namespace WHMS.Persistence.SeedData.Dto;

public class ShipmentsSeedDto
{
    public string CreatedBy { get; set; } = null!;
    public List<Shipment> Shipments { get; set; } = null!;
}

public class Shipment
{
    public string Warehouse { get; set; } = null!;
    public string Store { get; set; } = null!;
    public DateTime ExpectedSendingDate { get; set; }
    public DateTime SentAt { get; set; }
    public string SentBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public List<ShipmentItem> ShipmentItems { get; set; } = null!;
}

public class ShipmentItem
{
    public string SkuBarcode { get; set; } = null!;
    public int Quantity { get; set; }
}