using Microsoft.EntityFrameworkCore.Query.Internal;

namespace WHMS.Application.Features.Queries.Location.GetLocationData;

public class GetLocationDataResultCityDto
{
    public string CityId { get; set; } = null!;
    public string CityName { get; set; } = null!;
    public List<GetLocationDataResultDistrictDto> Districts { get; set; } = null!;
}