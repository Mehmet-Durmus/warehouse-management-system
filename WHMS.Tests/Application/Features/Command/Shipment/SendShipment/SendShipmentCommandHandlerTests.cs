using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Shipment.SendShipment;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Shipment.SendShipment;

public class SendShipmentCommandHandlerTests
{
    private readonly Mock<IShipmentRepository> _shipmentRepository = new();
    private readonly Mock<IStockStateRepository> _stockStateRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly SendShipmentCommandHandler _handler;
    private readonly Guid _shipmentId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _currentUserId = Guid.NewGuid();

    public SendShipmentCommandHandlerTests()
    {
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        _currentUserService.Setup(u => u.UserId).Returns(_currentUserId);
        _handler = new SendShipmentCommandHandler(_shipmentRepository.Object, _stockStateRepository.Object, _currentUserService.Object, _unitOfWork.Object);
    }

    private SendShipmentCommandRequest Request() => new() { ShipmentId = _shipmentId.ToString() };

    [Fact]
    public async Task Handle_ShipmentNotFound_ThrowsAndDoesNotCommit()
    {
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync((WHMS.Domain.Entities.Shipment)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Shipment not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShipmentBelongsToAnotherWarehouse_ThrowsSameNotFoundMessage()
    {
        var shipment = new WHMS.Domain.Entities.Shipment { Id = _shipmentId, WarehouseId = Guid.NewGuid() };
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Shipment not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_AlreadySent_ThrowsAndDoesNotTouchStock()
    {
        var shipment = new WHMS.Domain.Entities.Shipment { Id = _shipmentId, WarehouseId = _warehouseId, SendingDate = DateTime.Now };
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Shipment already sent.", exception.Message);
        _stockStateRepository.Verify(r => r.UpdateQuantity(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SufficientStockForAllItems_DecreasesStockAndMarksSent()
    {
        var item1 = new ShipmentItem { Id = Guid.NewGuid(), SkuId = Guid.NewGuid(), Quantity = 4 };
        var item2 = new ShipmentItem { Id = Guid.NewGuid(), SkuId = Guid.NewGuid(), Quantity = 2 };
        var shipment = new WHMS.Domain.Entities.Shipment { Id = _shipmentId, WarehouseId = _warehouseId, SendingDate = null, ShipmentItems = [item1, item2] };
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);
        _stockStateRepository.Setup(r => r.UpdateQuantity(_warehouseId, item1.SkuId, -4)).ReturnsAsync(1);
        _stockStateRepository.Setup(r => r.UpdateQuantity(_warehouseId, item2.SkuId, -2)).ReturnsAsync(1);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.NotNull(shipment.SendingDate);
        Assert.Equal(_currentUserId, shipment.SentById);
        Assert.Null(response.Errors);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_InsufficientStockForAnItem_ReturnsErrorsAndDoesNotCommit()
    {
        var sku = new SKU { Id = Guid.NewGuid(), SKUName = "kola", NormalizedSKUName = "KOLA", Barcode = "8690000000001" };
        var shortItem = new ShipmentItem { Id = Guid.NewGuid(), SkuId = sku.Id, SKU = sku, Quantity = 10 };
        var shipment = new WHMS.Domain.Entities.Shipment { Id = _shipmentId, WarehouseId = _warehouseId, SendingDate = null, ShipmentItems = [shortItem] };
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);
        _stockStateRepository.Setup(r => r.UpdateQuantity(_warehouseId, sku.Id, -10)).ReturnsAsync(0);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Equal(["Insufficient stock for kola."], response.Errors);
        // SendingDate/SentById are mutated in memory before the check, but since
        // commit never runs, nothing is actually persisted.
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NoItems_MarksSentWithoutTouchingStock()
    {
        var shipment = new WHMS.Domain.Entities.Shipment { Id = _shipmentId, WarehouseId = _warehouseId, SendingDate = null, ShipmentItems = null };
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.NotNull(shipment.SendingDate);
        _stockStateRepository.Verify(r => r.UpdateQuantity(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Null(response.Errors);
    }
}
