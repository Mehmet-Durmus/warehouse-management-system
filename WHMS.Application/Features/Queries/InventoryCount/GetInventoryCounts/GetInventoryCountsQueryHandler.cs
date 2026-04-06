using System.Text.RegularExpressions;
using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.DTOs.InventoryCount;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCounts;

public class GetInventoryCountsQueryHandler : IRequestHandler<GetInventoryCountsQueryRequest, GetInventoryCountsQueryResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetInventoryCountsQueryHandler(IInventoryCountRepository inventoryCountRepository, ICurrentUserService currentUserService)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetInventoryCountsQueryResponse> Handle(GetInventoryCountsQueryRequest request, CancellationToken cancellationToken)
    {
        var roles = _currentUserService.Roles;
        InventoryCountFilter filter = new()
        {
            SkuIds = request.SkuIds,
            Page = request.Page,
            PageSize = request.PageSize
        };

        if (roles!.Contains(ApplicationRole.LogisticDirector))
        {
            filter.WarehouseId = request.WarehouseId;
            filter.IsDone = request.IsDone;
            filter.CreatedAfter = request.CreatedAfter;
            filter.CreatedBefore = request.CreatedBefore;
            filter.UpdatedAfter = request.UpdatedAfter;
            filter.UpdatedBefore = request.UpdatedBefore;
        }
        else if (roles!.Contains(ApplicationRole.WarehouseManager))
        {
            filter.WarehouseId = _currentUserService.WarehouseId;
            filter.IsDone = request.IsDone;
            filter.CreatedAfter = request.CreatedAfter;
            filter.CreatedBefore = request.CreatedBefore;
            filter.UpdatedAfter = request.UpdatedAfter;
            filter.UpdatedBefore = request.UpdatedBefore;
        }
        else
        {
            filter.WarehouseId = _currentUserService.WarehouseId;
            filter.IsDone = false;
        }

        int count = await _inventoryCountRepository.CountInventoryCounts(filter);
        var inventoryCounts = await _inventoryCountRepository.GetInventoryCounts(filter, applyPagination: true);

        GetInventoryCountsQueryResponse response = new() { InventoryCounts = [] };
        response.Pagination = new(
            request.Page,
            request.PageSize,
            (int)Math.Ceiling((double)count / request.PageSize)
        );

        foreach (var inventoryCount in inventoryCounts)
            response.InventoryCounts.Add(new() 
            {
                InventoryCountId = inventoryCount.Id.ToString(),
                WarehouseId = inventoryCount.WarehouseId.ToString(),
                IsCompleted = inventoryCount.IsCompleted,
                CreatedAt = inventoryCount.CreatedAt,
                CreatedById = inventoryCount.CreatedById.ToString(),
            });

        return response;
    }
}