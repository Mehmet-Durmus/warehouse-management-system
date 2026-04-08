namespace WHMS.Persistence.SeedData.Dto;

public class WarehouseSeedDto
{
    public string CreatedBy { get; set; } = null!;
    public List<Warehouse> Warehouses { get; set; } = null!;
}

public class Warehouse
{
    public string WarehouseName { get; set; } = null!;
    public string City { get; set; } = null!;
    public string District { get; set; } = null!;
    public string Neighborhood { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string AddressLine { get; set; } = null!;
}