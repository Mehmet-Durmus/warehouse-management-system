namespace WHMS.Domain.Entities;

public class District
{
    public Guid DistrictId { get; set; }
    public required string Name { get; set; }
    public Guid CityId { get; set; }
    public required City City { get; set; }
    public List<Neighborhood>? Neighborhoods { get; set; }
}