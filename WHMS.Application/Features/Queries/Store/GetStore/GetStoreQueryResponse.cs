using WHMS.Domain.ValueObjects;

namespace WHMS.Application.Features.Queries.Store.GetStore;

public class GetStoreQueryResponse
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