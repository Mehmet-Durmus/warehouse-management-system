using MediatR;

namespace WHMS.Application.Features.Command.Warehouse.AssignEmployees;

public class AssignEmployeesCommandRequest : IRequest<AssignEmployeesCommandResponse>
{
    public string WarehouseId { get; set; } = null!;
    public string? ManagerId { get; set; }
    public List<string>? StaffIds { get; set; }
}