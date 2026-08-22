using MediatR;
using Microsoft.VisualBasic;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Filters;

namespace WHMS.Application.Features.Command.WasteRecord.UpdateWasteRecord;

public class UpdateWasteRecordCommandHandler : IRequestHandler<UpdateWasteRecordCommandRequest, UpdateWasteRecordCommandResponse>
{
    private readonly IWasteRecordRepository _wasteRecordRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInventoryCountRepository _inventoryCountRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICatalogRepository _catalogRepository;

    public UpdateWasteRecordCommandHandler(IWasteRecordRepository wasteRecordRepository, IUnitOfWork unitOfWork, IInventoryCountRepository inventoryCountRepository, ICurrentUserService currentUserService, ICatalogRepository catalogRepository)
    {
        _wasteRecordRepository = wasteRecordRepository;
        _unitOfWork = unitOfWork;
        _inventoryCountRepository = inventoryCountRepository;
        _currentUserService = currentUserService;
        _catalogRepository = catalogRepository;
    }

    public async Task<UpdateWasteRecordCommandResponse> Handle(UpdateWasteRecordCommandRequest request, CancellationToken cancellationToken)
    {
        var wasteRecord = await _wasteRecordRepository.GetWasteRecord(Guid.Parse(request.WasteRecordId!));
        
        bool notFound = wasteRecord switch
        {
            null => true,
            not null when _currentUserService.Roles!.Contains(ApplicationRole.WarehouseManager) 
                && wasteRecord.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains(ApplicationRole.WarehouseStaff)
                && (wasteRecord.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) 
                || wasteRecord.CreatedById != _currentUserService.UserId) => true,
            _ => false
        };
        
        if (notFound)
            throw new Exception("Waste record not found.");

        InventoryCountFilter uncompletedFilter = new()
        {
            WarehouseId = _currentUserService.WarehouseId,
            IsDone = false,
            CreatedAfter = wasteRecord!.CreatedAt
        };
        var uncompletedCounts = await _inventoryCountRepository.GetInventoryCounts(uncompletedFilter, applyPagination: false);
        if (uncompletedCounts.Count > 0)
            throw new Exception("Cannot update waste record while an active inventory count exists after its creation date.");

        InventoryCountFilter completedFilter = new()
        {
            WarehouseId = _currentUserService.WarehouseId,
            IsDone = true,
            SkuIds = new List<string> {request.SkuId!},
            CreatedAfter = wasteRecord.CreatedAt
        };
        var completedCounts = await _inventoryCountRepository.GetInventoryCounts(completedFilter, applyPagination: false);
        if (completedCounts.Count > 0)
            throw new Exception("Waste record cannot be updated because a completed inventory count including this SKU exists after its creation date.");

        var sku = await _catalogRepository.GetSku(Guid.Parse(request.SkuId!));
        if (sku is null)
            throw new Exception("Sku not found.");

        wasteRecord.SkuId = Guid.Parse(request.SkuId!);
        wasteRecord.Quantity = request.Quantity;
        wasteRecord.Description = request.Description;

        await _unitOfWork.CommitAsync();
        
        return new()
        {
            SkuId = wasteRecord.SkuId.ToString(),
            SkuName = sku.SKUName,
            Quantity = wasteRecord.Quantity,
            Description = wasteRecord.Description
        };
    }
}