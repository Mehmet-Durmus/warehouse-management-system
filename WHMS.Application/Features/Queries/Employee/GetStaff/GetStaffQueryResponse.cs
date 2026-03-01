using WHMS.Application.DTOs.Employee;

namespace WHMS.Application.Features.Queries.Employee.GetStaff;

public class GetStaffQueryReponse
{
    public List<UserDto>? Staff { get; set; }
}