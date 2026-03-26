namespace WHMS.Domain.ValueObjects;

public sealed record Address
{
    public Guid CityId { get; init; }
    public Guid DistrictId { get; init; }
    public Guid NeighborhoodId { get; init; }
    public string? PostalCode { get; init; } 
    public string? AddressLine { get; init; }

    public Address(string cityId, string districtId, string neighborhoodId, string postalCode, string addressLine)
    {
        CityId = Guid.Parse(cityId);
        DistrictId = Guid.Parse(districtId);
        NeighborhoodId = Guid.Parse(neighborhoodId);
        AddressLine = addressLine;
        PostalCode = postalCode;
    }

    private Address() {}
}