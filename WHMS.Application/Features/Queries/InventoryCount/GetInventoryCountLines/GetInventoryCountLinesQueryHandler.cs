using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.DTOs.InventoryCount;

namespace WHMS.Application.Features.Queries.InventoryCount.GetInventoryCountLines;

public class GetInventoryCountLinesQueryHandler : IRequestHandler<GetInventoryCountLinesQueryRequest, GetInventoryCountLinesQueryResponse>
{
    private readonly IInventoryCountRepository _inventoryCountRepository;

    public GetInventoryCountLinesQueryHandler(IInventoryCountRepository inventoryCountRepository)
    {
        _inventoryCountRepository = inventoryCountRepository;
    }

    public async Task<GetInventoryCountLinesQueryResponse> Handle(GetInventoryCountLinesQueryRequest request, CancellationToken cancellationToken)
    {
        InventoryCountLineFilter filter = new()
        {
            InventoryCountId = request.InventoryCountId,
            SkuId = request.SkuId,
            MinQuantity = request.MinQuantity,
            MaxQuantity = request.MaxQuantity,
            Page = request.Page,
            PageSize = request.PageSize
        };

        int count = await _inventoryCountRepository.CountInventoryCountLines(filter);
        var inventoryCountLines = await _inventoryCountRepository.GetInventoryCountLines(filter, applyPagination: true);

        GetInventoryCountLinesQueryResponse response = new()
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPage = (int)Math.Ceiling((double)count / request.PageSize),
            InventoryCountLines = []
        };

        foreach (var inventoryCountLine in inventoryCountLines)
            response.InventoryCountLines.Add(new InventoryCountLineDto
            {
                InventoryCountLineId = inventoryCountLine.Id.ToString(),
                InventoryCountId = inventoryCountLine.InventoryCountId.ToString(),
                SkuId = inventoryCountLine.SkuId.ToString(),
                Quantity = inventoryCountLine.Quantity,
                CreatedAt = inventoryCountLine.CreatedAt,
                CreatedById = inventoryCountLine.CreatedById.ToString()
            });
        
        return response;
    }
}