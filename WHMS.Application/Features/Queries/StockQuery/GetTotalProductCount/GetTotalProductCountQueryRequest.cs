using MediatR;

namespace WHMS.Application.Features.Queries.StockQuery.GetTotalProductCount;

public class GetTotalProductCountQueryRequest : IRequest<GetTotalProductCountQueryResponse>
{
    public string? WarehouseId { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public string? NeighborhodId { get; set; }
}