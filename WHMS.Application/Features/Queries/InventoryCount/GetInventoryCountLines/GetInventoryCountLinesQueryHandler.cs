using MediatR;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.DTOs.InventoryCount;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCountLines;

public class GetInventoryCountLinesQueryHandler : IRequestHandler<GetInventoryCountLinesQueryRequest, GetInventoryCountLinesQueryResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetInventoryCountLinesQueryHandler(IInventoryCountRepository inventoryCountRepository, ICurrentUserService currentUserService)
    {
        _inventoryCountRepository = inventoryCountRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetInventoryCountLinesQueryResponse> Handle(GetInventoryCountLinesQueryRequest request, CancellationToken cancellationToken)
    {
        var inventoryCount = await _inventoryCountRepository.GetInventoryCount(Guid.Parse(request.InventoryCountId!));
        bool notFound = inventoryCount switch
        {
            null => true,
            not null when _currentUserService.Roles!.Contains("WarehouseManager")
                && inventoryCount.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains("WarehouseStaff")
                && inventoryCount.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains("WarehouseStaff")
                && inventoryCount.WarehouseId == Guid.Parse(_currentUserService.WarehouseId!)
                && !inventoryCount.IsCompleted => true,
            _ => false
        };

        if (notFound)
            throw new Exception("Inventory count not found.");

        InventoryCountLineFilter filter = new()
        {
            InventoryCountId = request.InventoryCountId,
            SkuId = request.SkuId,
            MinQuantity = request.MinQuantity,
            MaxQuantity = request.MaxQuantity,
            MinVariance = request.MinVariance,
            MaxVariance = request.MaxVariance,
            Page = request.Page,
            PageSize = request.PageSize
        };

        int count = await _inventoryCountRepository.CountInventoryCountLines(filter);
        var inventoryCountLines = await _inventoryCountRepository.GetInventoryCountLines(filter, applyPagination: true);

        GetInventoryCountLinesQueryResponse response = new() { InventoryCountLines = [] };
        response.Pagination = new(
            request.Page,
            request.PageSize,
            (int)Math.Ceiling((double) count / request.PageSize)
        );

        foreach (var inventoryCountLine in inventoryCountLines)
            response.InventoryCountLines.Add(new()
            {
                InventoryCountLineId = inventoryCountLine.Id.ToString(),
                SkuId = inventoryCountLine.SkuId.ToString(),
                SkuName = inventoryCountLine.Sku!.SKUName,
                Quantity = inventoryCountLine.Quantity,
                Variance = inventoryCountLine.Variance,
                CreatedAt = inventoryCountLine.CreatedAt,
                CreatedById = inventoryCountLine.CreatedById.ToString()!,
                CreatedByName = inventoryCountLine.CreatedByName!
            });
        
        return response;
    }
}