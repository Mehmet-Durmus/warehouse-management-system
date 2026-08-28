using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.Catalog;
using WHMS.Domain.BusinessRules.WasteRecord;

namespace WHMS.Application.Features.Command.WasteRecord.CreateWasteRecord;

public class CreateWasteRecordCommandHandler : IRequestHandler<CreateWasteRecordCommandRequest, CreateWasteRecordCommandResponse>
{
    private readonly IWasteRecordRepository _wasteRecordRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStockStateRepository _stockStateRepository;
    private readonly ICatalogRepository _catalogRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateWasteRecordCommandHandler(IWasteRecordRepository wasteRecordRepository, IUnitOfWork unitOfWork, IStockStateRepository stockStateRepository, ICatalogRepository catalogRepository, ICurrentUserService currentUserService)
    {
        _wasteRecordRepository = wasteRecordRepository;
        _unitOfWork = unitOfWork;
        _stockStateRepository = stockStateRepository;
        _catalogRepository = catalogRepository;
        _currentUserService = currentUserService;
    }

    public async Task<CreateWasteRecordCommandResponse> Handle(CreateWasteRecordCommandRequest request, CancellationToken cancellationToken)
    {
        var sku = SkuRules.EnsureExists(await _catalogRepository.GetSku(Guid.Parse(request.SkuId!)));

        Domain.Entities.WasteRecord wasteRecord = new()
        {
            WarehouseId = Guid.Parse(_currentUserService.WarehouseId!),
            SkuId = Guid.Parse(request.SkuId!),
            Quantity = request.Quantity ?? 0,
            Description = request.Description
        };

        await _wasteRecordRepository.CreateWasteRecord(wasteRecord);
        var affectedRow = await _stockStateRepository.UpdateQuantity(wasteRecord.WarehouseId, wasteRecord.SkuId, -wasteRecord.Quantity);
        WasteRecordRules.EnsureSufficientStock(affectedRow != 0);
        await _unitOfWork.CommitAsync();

        return new()
        {
            WasteRecordId = wasteRecord.Id.ToString(),
            SkuId = wasteRecord.SkuId.ToString(),
            SkuName = sku.SKUName,
            Quantity = wasteRecord.Quantity,
            Description = wasteRecord.Description
        };
    }
}