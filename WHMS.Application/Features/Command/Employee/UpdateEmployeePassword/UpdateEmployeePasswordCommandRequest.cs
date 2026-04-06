using MediatR;

namespace WHMS.Application.Features.Command.Employee.UpdateEmployeePassword;

public class UpdateEmployeePasswordCommandRequest : IRequest<UpdateEmployeePasswordCommandResponse>
{
    public string? EmployeeId { get; set; }
}