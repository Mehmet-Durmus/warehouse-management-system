namespace WHMS.Domain.Entities;

public class Neighborhood
{
    public Guid NeighborhoodId { get; set; }
    public required string Name { get; set; }
    public Guid DistrictId { get; set; }
    public required District District { get; set; }
}