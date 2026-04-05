using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;

namespace WHMS.Application.Features.Queries.Warehouse.GetWarehouseCount;

public class GetWarehouseCountQueryHandler : IRequestHandler<GetWarehouseCountQueryRequest, GetWarehouseCountQueryResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;

    public GetWarehouseCountQueryHandler(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<GetWarehouseCountQueryResponse> Handle(GetWarehouseCountQueryRequest request, CancellationToken cancellationToken)
    {
        int warehouseCount = await _warehouseRepository.GetWarehousesCount(new WarehouseFilter());
        return new() { WarehouseCount = warehouseCount };
    }
}