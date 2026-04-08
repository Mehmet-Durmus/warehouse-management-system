namespace WHMS.Persistence.SeedData.Dto;

public class LocationSeedDto
{
    public List<City> Cities { get; set; } = null!;
}

public class City
{
    public string Name { get; set; } = null!;
    public List<District> Districts { get; set; } = null!;
}
public class District
{
    public string Name { get; set; } = null!;
    public List<Neighborhood> Neighborhoods { get; set; } = null!;
}
public class Neighborhood
{
    public string Name { get; set; } = null!;
}