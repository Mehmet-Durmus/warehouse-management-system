using MediatR;

namespace WHMS.Application.Features.Queries.Warehouse.GetWarehouse;

public class GetWarehouseQueryRequest : IRequest<GetWarehouseQueryResponse>
{
    public string? WarehouseId { get; set; }
}