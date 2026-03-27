using MediatR;

namespace WHMS.Application.Features.Queries.Employee.GetAllEmployees;

public class GetAllEmployeesQueryRequest : IRequest<GetAllEmployeesQueryResponse>
{
    public string? Name { get; set; }
    public string? WarehouseId { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public string? NeighborhoodId { get; set; }
    public bool? IsAssignedToWarehouse { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? UpdatedAfter { get; set; }
    public DateTime? UpdatedBefore { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}