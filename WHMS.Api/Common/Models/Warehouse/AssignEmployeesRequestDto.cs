namespace WHMS.Api.Common.Models.Warehouse;

public class AssignEmployeesRequestDto
{
    public string? ManagerId { get; set; }
    public List<string>? StaffIds { get; set; }
}