namespace WHMS.Persistence.SeedData.Dto;

public class StockStatesSeedDto
{
    public List<StockState> StockStates { get; set; } = null!;
}

public class StockState
{
    public string Warehouse { get; set; } = null!;
    public List<Data> Data { get; set; } = null!;
}

public class Data
{
    public string Sku { get; set; } = null!;
    public int Quantity { get; set; }
}