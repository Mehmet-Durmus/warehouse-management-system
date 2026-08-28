using MediatR;
using Microsoft.VisualBasic;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.BusinessRules.Catalog;
using WHMS.Domain.BusinessRules.WasteRecord;

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
        bool currentUserIsManager = _currentUserService.Roles?.Contains(ApplicationRole.WarehouseManager) ?? false;
        bool currentUserIsStaff = _currentUserService.Roles?.Contains(ApplicationRole.WarehouseStaff) ?? false;
        Guid? currentUserWarehouseId = _currentUserService.WarehouseId is null ? null : Guid.Parse(_currentUserService.WarehouseId);

        var wasteRecord = WasteRecordRules.EnsureAccessible(
            await _wasteRecordRepository.GetWasteRecord(Guid.Parse(request.WasteRecordId!)),
            currentUserIsManager, currentUserIsStaff, currentUserWarehouseId, _currentUserService.UserId);

        InventoryCountFilter uncompletedFilter = new()
        {
            WarehouseId = _currentUserService.WarehouseId,
            IsDone = false,
            CreatedAfter = wasteRecord.CreatedAt
        };
        var uncompletedCounts = await _inventoryCountRepository.GetInventoryCounts(uncompletedFilter, applyPagination: false);
        WasteRecordRules.EnsureNoActiveInventoryCountBlocksUpdate(uncompletedCounts.Count > 0);

        InventoryCountFilter completedFilter = new()
        {
            WarehouseId = _currentUserService.WarehouseId,
            IsDone = true,
            SkuIds = new List<string> {request.SkuId!},
            CreatedAfter = wasteRecord.CreatedAt
        };
        var completedCounts = await _inventoryCountRepository.GetInventoryCounts(completedFilter, applyPagination: false);
        WasteRecordRules.EnsureNoCompletedInventoryCountWithSkuBlocksUpdate(completedCounts.Count > 0);

        var sku = SkuRules.EnsureExists(await _catalogRepository.GetSku(Guid.Parse(request.SkuId!)));

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