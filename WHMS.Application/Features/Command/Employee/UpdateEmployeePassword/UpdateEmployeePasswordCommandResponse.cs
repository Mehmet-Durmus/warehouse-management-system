namespace WHMS.Application.Features.Command.Employee.UpdateEmployeePassword;

public class UpdateEmployeePasswordCommandResponse
{
    public UpdateEmployeePasswordResultSuccessDto? Result { get; set; }
    public List<string>? Errors { get; set; } 
}