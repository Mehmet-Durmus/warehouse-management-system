namespace WHMS.Domain.Entities;

public class City
{
    public Guid CityId { get; set; }
    public required string Name { get; set; }
    public List<District>? Districts { get; set; }
}