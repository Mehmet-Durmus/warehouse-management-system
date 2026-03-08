namespace WHMS.Application.DTOs.Store;

public class StoreDto
{
    public string? StoreName { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Neighborhood { get; set; }
    public string? AddressLine { get; set; }
    public string? PostalCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}