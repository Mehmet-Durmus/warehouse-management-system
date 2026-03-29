using MediatR;

namespace WHMS.Application.Features.Queries.Store.GetStores;

public class GetStoresQueryRequest : IRequest<GetStoresQueryResponse>
{
    public string? Name { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public string? NeighborhoodId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}