namespace WHMS.Application.Features.Queries.Location.GetLocationData;

public class GetLocationDataResultDistrictDto
{
    public string DistrictId { get; set; } = null!;
    public string DistrictName { get; set; } = null!;
    public List<GetLocationDataResultNeighborhoodDto> Neighborhoods { get; set; } = null!;
}