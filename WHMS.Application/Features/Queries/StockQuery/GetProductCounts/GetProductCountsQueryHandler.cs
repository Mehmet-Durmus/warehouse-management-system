using System.Runtime.Intrinsics.X86;
using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Filters;

namespace WHMS.Application.Features.Queries.StockQuery.GetProductCounts;

public class GetProductCountsQueryHandler : IRequestHandler<GetProductCountsQueryRequest, GetProductCountsQueryResponse>
{
    private readonly IStockStateRepository _stockStateRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetProductCountsQueryHandler(IStockStateRepository stockStateRepository, ICurrentUserService currentUserService)
    {
        _stockStateRepository = stockStateRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetProductCountsQueryResponse> Handle(GetProductCountsQueryRequest request, CancellationToken cancellationToken)
    {
        ProductCountFilter filter = new();
        if (_currentUserService.Roles!.Contains(ApplicationRole.LogisticDirector))
        {
            filter.WarehouseId = request.WarehouseId;
            filter.CityId = request.CityId;
            filter.DistrictId = request.DistrictId;
            filter.NeighborhodId = request.NeighborhodId;
        }
        else
            filter.WarehouseId = _currentUserService.WarehouseId;
        
        var stockStates = await _stockStateRepository.GetProductCounts(filter);
        var response = new GetProductCountsQueryResponse
            {
                Warehouses = stockStates
                    .GroupBy(ss => ss.Warehouse)
                    .Select(wg => new GetProductCountsResultWarehouseDto
                    {
                        WarehouseId = wg.Key!.Id.ToString(),
                        WarehouseName = wg.Key.WarehouseName,
                        Quantity = wg.Sum(ss => ss.Quantity),
                        Categories = wg
                            .GroupBy(ss => ss.Sku!.Category)
                            .Select(cg => new GetProductCountsResultCategoryDto
                            {
                                CategoryId = cg.Key.Id.ToString(),
                                CategoryName = cg.Key.CategoryName,
                                Quantity = cg.Sum(ss => ss.Quantity),
                                Skus = cg.Select(ss => new GetProductCountsResultSkuDto
                                {
                                    SkuId = ss.Sku!.Id.ToString(),
                                    SkuName = ss.Sku.SKUName,
                                    Quantity = ss.Quantity
                                }).ToList()
                            }).ToList()
                    }).ToList(),
                TotalQuantity = stockStates.Sum(ss => ss.Quantity)
            };

        return response;
    }
}