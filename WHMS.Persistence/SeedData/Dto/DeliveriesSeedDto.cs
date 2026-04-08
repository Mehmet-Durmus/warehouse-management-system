namespace WHMS.Persistence.SeedData.Dto;

public class DeliveriesSeedDto
{
    public List<Deliveries> Deliveries { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
}

public class Deliveries
{
    public string Warehouse { get; set; } = null!;
    public DateTime ExpectedArrivalDate { get; set; }
    public DateTime ReceivedAt { get; set; }
    public string ReceivedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public List<DeliveryItems> DeliveryItems { get; set; } = null!;
}

public class DeliveryItems
{
    public string SkuBarcode { get; set; } = null!;
    public int Quantity { get; set; }
}