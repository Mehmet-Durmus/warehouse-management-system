using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.DTOs.InventoryCount;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCounts;

public class GetInventoryCountsQueryHandler : IRequestHandler<GetInventoryCountsQueryRequest, GetInventoryCountsQueryResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;

    public GetInventoryCountsQueryHandler(IInventoryCountRepository inventoryCountRepository)
    {
        _inventoryCountRepository = inventoryCountRepository;
    }

    public async Task<GetInventoryCountsQueryResponse> Handle(GetInventoryCountsQueryRequest request, CancellationToken cancellationToken)
    {
        InventoryCountFilter filter = new()
        {
            WarehouseId = request.WarehouseId,
            Page = request.Page,
            PageSize = request.PageSize
        };

        int count = await _inventoryCountRepository.CountInventoryCounts(filter);
        var inventoryCounts = await _inventoryCountRepository.GetInventoryCounts(filter, applyPagination: true);

        GetInventoryCountsQueryResponse response = new()
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPage = (int)Math.Ceiling((double)count / request.PageSize),
            InventoryCounts = []
        };

        foreach (var inventoryCount in inventoryCounts)
            response.InventoryCounts.Add(new InventoryCountDto
            {
                InventoryCountId = inventoryCount.Id.ToString(),
                WarehouseId = inventoryCount.WarehouseId.ToString(),
                CreatedAt = inventoryCount.CreatedAt,
                CreatedById = inventoryCount.CreatedById.ToString(),
            });

        return response;
    }
}