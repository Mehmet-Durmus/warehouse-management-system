using WHMS.Application.DTOs.Employee;

namespace WHMS.Application.Features.Queries.Employee.GetManagers;

public class GetManagersQueryResponse
{
    public List<UserDto>? Employees { get; set; }
}