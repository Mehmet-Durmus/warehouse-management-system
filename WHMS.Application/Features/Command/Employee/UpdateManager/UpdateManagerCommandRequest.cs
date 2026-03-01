using MediatR;

namespace WHMS.Application.Features.Command.Employee.UpdateManager;

public class UpdateManagerCommandRequest : IRequest<UpdateManagerCommandResponse>
{
    public string? UserId { get; set; }
    public string? FullName { get; set; }
    public string? WarehouseId { get; set; }
}