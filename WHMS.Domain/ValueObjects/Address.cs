namespace WHMS.Domain.ValueObjects;

public sealed record Address
{
    public Guid CityId { get; init; }
    public Guid DistrictId { get; init; }
    public Guid NeighborhoodId { get; init; }
    public required string PostalCode { get; init; } 
    public required string AddressLine { get; init; }
}