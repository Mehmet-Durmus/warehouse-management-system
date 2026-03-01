using MediatR;

namespace WHMS.Application.Features.Command.Employee.DeleteEmployee;

public class DeleteEmployeeCommandRequest : IRequest<DeleteEmployeeCommandResponse>
{
    public string? EmployeeId { get; set; }
}