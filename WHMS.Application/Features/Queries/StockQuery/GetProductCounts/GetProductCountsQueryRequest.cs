using MediatR;

namespace WHMS.Application.Features.Queries.StockQuery.GetProductCounts;

public class GetProductCountsQueryRequest : IRequest<GetProductCountsQueryResponse>
{
    public string? WarehouseId { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public string? NeighborhodId { get; set; }
}