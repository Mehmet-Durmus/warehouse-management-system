using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Delivery.UpdateDeliveryItem;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Delivery.UpdateDeliveryItem;

public class UpdateDeliveryItemCommandHandlerTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly UpdateDeliveryItemCommandHandler _handler;
    private readonly Guid _deliveryItemId = Guid.NewGuid();
    private readonly Guid _deliveryId = Guid.NewGuid();
    private readonly Guid _skuId = Guid.NewGuid();

    public UpdateDeliveryItemCommandHandlerTests()
    {
        _handler = new UpdateDeliveryItemCommandHandler(_deliveryRepository.Object, _unitOfWork.Object, _catalogRepository.Object);
    }

    private UpdateDeliveryItemCommandRequest Request() => new()
    {
        DeliveryItemId = _deliveryItemId.ToString(),
        DeliveryId = _deliveryId.ToString(),
        SkuId = _skuId.ToString(),
        Quantity = 15
    };

    [Fact]
    public async Task Handle_DeliveryItemNotFound_ThrowsAndDoesNotCommit()
    {
        _deliveryRepository.Setup(r => r.GetDeliveryItem(_deliveryItemId)).ReturnsAsync((DeliveryItem)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery item not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ParentDeliveryNotFound_Throws()
    {
        var item = new DeliveryItem { Id = _deliveryItemId, DeliveryId = _deliveryId, SkuId = _skuId, Quantity = 5 };
        _deliveryRepository.Setup(r => r.GetDeliveryItem(_deliveryItemId)).ReturnsAsync(item);
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync((WHMS.Domain.Entities.Delivery)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_ParentDeliveryAlreadyReceived_ThrowsWithoutCheckingSku()
    {
        var item = new DeliveryItem { Id = _deliveryItemId, DeliveryId = _deliveryId, SkuId = _skuId, Quantity = 5 };
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, ReceivedAt = DateTime.Now };
        _deliveryRepository.Setup(r => r.GetDeliveryItem(_deliveryItemId)).ReturnsAsync(item);
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery has already been received.", exception.Message);
        _catalogRepository.Verify(r => r.GetSku(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NewSkuNotFound_ThrowsAndLeavesItemUnchanged()
    {
        var item = new DeliveryItem { Id = _deliveryItemId, DeliveryId = _deliveryId, SkuId = Guid.NewGuid(), Quantity = 5 };
        var originalSkuId = item.SkuId;
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, ReceivedAt = null };
        _deliveryRepository.Setup(r => r.GetDeliveryItem(_deliveryItemId)).ReturnsAsync(item);
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync((SKU)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Sku not found.", exception.Message);
        Assert.Equal(originalSkuId, item.SkuId);
        Assert.Equal(5, item.Quantity);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesItemAndReturnsResponse()
    {
        var item = new DeliveryItem { Id = _deliveryItemId, DeliveryId = _deliveryId, SkuId = Guid.NewGuid(), Quantity = 5 };
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, ReceivedAt = null };
        var sku = new SKU { Id = _skuId, SKUName = "kola", NormalizedSKUName = "KOLA", Barcode = "8690000000001" };
        _deliveryRepository.Setup(r => r.GetDeliveryItem(_deliveryItemId)).ReturnsAsync(item);
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync(sku);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Equal(_skuId, item.SkuId);
        Assert.Equal(15, item.Quantity);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("kola", response.SkuName);
        Assert.Equal(15, response.Quantity);
    }
}
