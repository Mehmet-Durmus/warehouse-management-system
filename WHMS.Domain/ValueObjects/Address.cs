namespace WHMS.Domain.ValueObjects;

public sealed record Address
{
    public Guid CityId { get; init; }
    public Guid DistrictId { get; init; }
    public Guid NeighborhoodId { get; init; }
    public string? PostalCode { get; init; } 
    public string? AddressLine { get; init; }

    public Address(Guid cityId, Guid districtId, Guid neighborhoodId, string postalCode, string addressLine)
    {
        CityId = cityId;
        DistrictId = districtId;
        NeighborhoodId = neighborhoodId;
        AddressLine = addressLine;
        PostalCode = postalCode;
    }

    private Address() {}
}