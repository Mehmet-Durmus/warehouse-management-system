using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Catalog.DeleteSku;

namespace WHMS.Tests.Application.Features.Command.Catalog.DeleteSku;

public class DeleteSkuCommandHandlerTests
{
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IStockStateRepository> _stockStateRepository = new();
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly DeleteSkuCommandHandler _handler;
    private readonly Guid _skuId = Guid.NewGuid();

    public DeleteSkuCommandHandlerTests()
    {
        _handler = new DeleteSkuCommandHandler(
            _catalogRepository.Object, _unitOfWork.Object, _stockStateRepository.Object, _deliveryRepository.Object);
    }

    [Fact]
    public async Task Handle_SkuHasNoStockOrPendingDeliveries_DeletesAndCommits()
    {
        _stockStateRepository.Setup(r => r.HasStockInAnyWarehouse(_skuId)).ReturnsAsync(false);
        _deliveryRepository.Setup(r => r.HasIncomingDeliveriesWithSku(_skuId)).ReturnsAsync(false);

        var request = new DeleteSkuCommandRequest { SkuId = _skuId.ToString() };

        await _handler.Handle(request, CancellationToken.None);

        _catalogRepository.Verify(r => r.DeleteSku(_skuId), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_SkuHasStockInAWarehouse_ThrowsAndDoesNotDelete()
    {
        _stockStateRepository.Setup(r => r.HasStockInAnyWarehouse(_skuId)).ReturnsAsync(true);
        _deliveryRepository.Setup(r => r.HasIncomingDeliveriesWithSku(_skuId)).ReturnsAsync(false);

        var request = new DeleteSkuCommandRequest { SkuId = _skuId.ToString() };

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Cannot delete SKU because it exists in stock or pending deliveries.", exception.Message);
        _catalogRepository.Verify(r => r.DeleteSku(It.IsAny<Guid>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_SkuHasPendingDelivery_ThrowsAndDoesNotDelete()
    {
        _stockStateRepository.Setup(r => r.HasStockInAnyWarehouse(_skuId)).ReturnsAsync(false);
        _deliveryRepository.Setup(r => r.HasIncomingDeliveriesWithSku(_skuId)).ReturnsAsync(true);

        var request = new DeleteSkuCommandRequest { SkuId = _skuId.ToString() };

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Cannot delete SKU because it exists in stock or pending deliveries.", exception.Message);
        _catalogRepository.Verify(r => r.DeleteSku(It.IsAny<Guid>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }
}
