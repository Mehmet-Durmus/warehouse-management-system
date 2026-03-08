using MediatR;

namespace WHMS.Application.Features.Queries.Warehouse.GetAllWarehouses;

public class GetAllWarehousesQueryRequest : IRequest<GetAllWarehousesQueryResponse>
{
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}