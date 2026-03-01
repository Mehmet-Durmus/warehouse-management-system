using MediatR;

namespace WHMS.Application.Features.Command.Employee.CreateEmployee;

public class CreateManagerCommandRequest : IRequest<CreateManagerCommandResponse>
{
    public string? FullName { get; set; }
    public string? WarehouseId { get; set; }
}