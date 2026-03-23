using MediatR;

namespace WHMS.Application.Features.Queries.Warehouse.GetAllWarehouses;

public class GetAllWarehousesQueryRequest : IRequest<GetAllWarehousesQueryResponse>
{
    public string? Name { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public string? NeighborhoodId { get; set; }
    public List<string>? SkuIds { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? UpdatedAfter { get; set; }
    public DateTime? UpdatedBefore { get; set; }
}