using MediatR;

namespace WHMS.Application.Features.Command.Warehouse.DeleteWarehouse;

public class DeleteWarehouseCommandRequest : IRequest<DeleteWarehouseCommandResponse>
{
    public required string WarehouseId { get; set; }
}