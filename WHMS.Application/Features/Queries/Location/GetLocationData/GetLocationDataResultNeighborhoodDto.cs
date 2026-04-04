using Microsoft.EntityFrameworkCore.Query.Internal;

namespace WHMS.Application.Features.Queries.Location.GetLocationData;

public class GetLocationDataResultNeighborhoodDto
{
    public string NeighborhoodId { get; set; } = null!;
    public string NeighborhoodName { get; set; } = null!;
}