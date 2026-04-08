namespace WHMS.Persistence.SeedData.Dto;

public class StoreSeedDto
{
    public string CreatedBy { get; set; } = null!;
    public List<Store> Stores { get; set; } = null!;
}

public class Store
{
    public string StoreName { get; set; } = null!;
    public string City { get; set; } = null!;
    public string District { get; set; } = null!;
    public string Neighborhood { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string AddressLine { get; set; } = null!;
}