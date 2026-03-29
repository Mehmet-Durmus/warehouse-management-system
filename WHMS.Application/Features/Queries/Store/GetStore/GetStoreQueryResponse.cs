using WHMS.Domain.ValueObjects;

namespace WHMS.Application.Features.Queries.Store.GetStore;

public class GetStoreQueryResponse
{
    public string StoreName { get; set; } = null!;
    public string CityId { get; set; } = null!;
    public string DistrictId { get; set; } = null!;
    public string NeighborhoodId { get; set; } = null!;
    public string Address { get; set; } = null!;
}