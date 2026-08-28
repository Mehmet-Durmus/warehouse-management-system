using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.WasteRecord.CreateWasteRecord;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.WasteRecord.CreateWasteRecord;

public class CreateWasteRecordCommandHandlerTests
{
    private readonly Mock<IWasteRecordRepository> _wasteRecordRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IStockStateRepository> _stockStateRepository = new();
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly CreateWasteRecordCommandHandler _handler;
    private readonly Guid _skuId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();

    public CreateWasteRecordCommandHandlerTests()
    {
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        _handler = new CreateWasteRecordCommandHandler(
            _wasteRecordRepository.Object, _unitOfWork.Object, _stockStateRepository.Object,
            _catalogRepository.Object, _currentUserService.Object);
    }

    [Fact]
    public async Task Handle_SkuNotFound_ThrowsAndDoesNotPersist()
    {
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync((SKU)null!);

        var request = new CreateWasteRecordCommandRequest { SkuId = _skuId.ToString(), Quantity = 5 };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Sku not found.", exception.Message);
        _wasteRecordRepository.Verify(r => r.CreateWasteRecord(It.IsAny<WHMS.Domain.Entities.WasteRecord>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_SkuFoundAndStockIsSufficient_PersistsRecordAndDecreasesStock()
    {
        var sku = new SKU { Id = _skuId, SKUName = "kola", NormalizedSKUName = "KOLA", Barcode = "8690000000001" };
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync(sku);
        _stockStateRepository.Setup(r => r.UpdateQuantity(_warehouseId, _skuId, -5)).ReturnsAsync(1);

        var request = new CreateWasteRecordCommandRequest { SkuId = _skuId.ToString(), Quantity = 5, Description = "kirildi" };

        var response = await _handler.Handle(request, CancellationToken.None);

        _wasteRecordRepository.Verify(r => r.CreateWasteRecord(It.Is<WHMS.Domain.Entities.WasteRecord>(w =>
            w.WarehouseId == _warehouseId && w.SkuId == _skuId && w.Quantity == 5 && w.Description == "kirildi")), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("kola", response.SkuName);
        Assert.Equal(5, response.Quantity);
    }

    [Fact]
    public async Task Handle_StockIsInsufficient_ThrowsAfterAddingButBeforeCommitting()
    {
        var sku = new SKU { Id = _skuId, SKUName = "kola", NormalizedSKUName = "KOLA", Barcode = "8690000000001" };
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync(sku);
        _stockStateRepository.Setup(r => r.UpdateQuantity(_warehouseId, _skuId, -5)).ReturnsAsync(0);

        var request = new CreateWasteRecordCommandRequest { SkuId = _skuId.ToString(), Quantity = 5 };

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Insufficient stock exception.", exception.Message);
        // The record is added to the repository before the stock check runs; only
        // the final commit is skipped, so nothing actually reaches the database.
        _wasteRecordRepository.Verify(r => r.CreateWasteRecord(It.IsAny<WHMS.Domain.Entities.WasteRecord>()), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_QuantityNotProvided_DefaultsToZero()
    {
        var sku = new SKU { Id = _skuId, SKUName = "kola", NormalizedSKUName = "KOLA", Barcode = "8690000000001" };
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync(sku);
        _stockStateRepository.Setup(r => r.UpdateQuantity(_warehouseId, _skuId, 0)).ReturnsAsync(1);

        var request = new CreateWasteRecordCommandRequest { SkuId = _skuId.ToString(), Quantity = null };

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(0, response.Quantity);
    }
}
