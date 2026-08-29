using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Delivery.CreateDeliveryItem;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Delivery.CreateDeliveryItem;

public class CreateDeliveryItemCommandHandlerTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly CreateDeliveryItemCommandHandler _handler;
    private readonly Guid _deliveryId = Guid.NewGuid();
    private readonly Guid _skuId = Guid.NewGuid();

    public CreateDeliveryItemCommandHandlerTests()
    {
        _handler = new CreateDeliveryItemCommandHandler(_deliveryRepository.Object, _unitOfWork.Object, _catalogRepository.Object);
    }

    private CreateDeliveryItemCommandRequest Request() => new() { DeliveryId = _deliveryId.ToString(), SkuId = _skuId.ToString(), Quantity = 10 };

    [Fact]
    public async Task Handle_DeliveryNotFound_ThrowsAndDoesNotAdd()
    {
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync((WHMS.Domain.Entities.Delivery)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery not found.", exception.Message);
        _deliveryRepository.Verify(r => r.AddDeliveryItem(It.IsAny<DeliveryItem>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeliveryAlreadyReceived_ThrowsAndDoesNotAdd()
    {
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, ReceivedAt = DateTime.Now };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery has already been received.", exception.Message);
        _deliveryRepository.Verify(r => r.AddDeliveryItem(It.IsAny<DeliveryItem>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidRequest_AddsItemAndReturnsResponse()
    {
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, ReceivedAt = null };
        var sku = new SKU { Id = _skuId, SKUName = "kola", NormalizedSKUName = "KOLA", Barcode = "8690000000001" };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync(sku);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        _deliveryRepository.Verify(r => r.AddDeliveryItem(It.Is<DeliveryItem>(i =>
            i.DeliveryId == _deliveryId && i.SkuId == _skuId && i.Quantity == 10)), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("kola", response.SkuName);
        Assert.Equal(10, response.Quantity);
    }
}
