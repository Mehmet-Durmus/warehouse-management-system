using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;

namespace WHMS.Application.Features.Queries.StockQuery.GetTotalProductCount;

public class GetTotalProductCountQueryHandler : IRequestHandler<GetTotalProductCountQueryRequest, GetTotalProductCountQueryResponse>
{
    private readonly IStockStateRepository _stockStateRepository;

    public GetTotalProductCountQueryHandler(IStockStateRepository stockStateRepository)
    {
        _stockStateRepository = stockStateRepository;
    }

    public async Task<GetTotalProductCountQueryResponse> Handle(GetTotalProductCountQueryRequest request, CancellationToken cancellationToken)
    {
        ProductCountFilter filter = new()
        {
            WarehouseId = request.WarehouseId,
            CityId = request.CityId,
            DistrictId = request.DistrictId,
            NeighborhodId = request.NeighborhodId
        };
        int count = await _stockStateRepository.GetTotalProductCount(filter);

        return new() { Count = count };
    }
}