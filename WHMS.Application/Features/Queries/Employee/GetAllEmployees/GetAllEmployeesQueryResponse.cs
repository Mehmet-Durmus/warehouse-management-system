using WHMS.Application.Common.DTOs;

namespace WHMS.Application.Features.Queries.Employee.GetAllEmployees;

public class GetAllEmployeesQueryResponse
{
    public List<GetAllEmployeesResultUserDto>? Employees { get; set; }
    public PaginationDto Pagination { get; set; } = null!;
}