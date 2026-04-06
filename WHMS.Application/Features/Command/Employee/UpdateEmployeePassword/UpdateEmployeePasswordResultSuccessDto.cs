namespace WHMS.Application.Features.Command.Employee.UpdateEmployeePassword;

public class UpdateEmployeePasswordResultSuccessDto
{
    public string EmployeeId { get; set; } = null!;
    public string EmployeeUserName { get; set; } = null!;
    public string EmployeeFullName { get; set; } = null!;
    public string TempPassword { get; set; } = null!;}