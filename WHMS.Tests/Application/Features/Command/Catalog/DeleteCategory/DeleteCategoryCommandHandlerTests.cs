using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Catalog.DeleteCategory;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Command.Catalog.DeleteCategory;

public class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IStockStateRepository> _stockStateRepository = new();
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly DeleteCategoryCommandHandler _handler;
    private readonly Guid _categoryId = Guid.NewGuid();

    public DeleteCategoryCommandHandlerTests()
    {
        _handler = new DeleteCategoryCommandHandler(
            _catalogRepository.Object, _unitOfWork.Object, _stockStateRepository.Object, _deliveryRepository.Object);
    }

    private static SKU MakeSku(Guid categoryId) => new()
    {
        Id = Guid.NewGuid(),
        SKUName = "kola",
        NormalizedSKUName = "KOLA",
        Barcode = "8690000000001",
        CategoryId = categoryId
    };

    [Fact]
    public async Task Handle_CategoryHasNoSkus_DeletesCategoryAndCommits()
    {
        var category = new Category { Id = _categoryId, CategoryName = "market", NormalizedCategoryName = "MARKET", Skus = null };
        _catalogRepository.Setup(r => r.GetCategory(_categoryId)).ReturnsAsync(category);

        var request = new DeleteCategoryCommandRequest { CategoryId = _categoryId.ToString() };

        await _handler.Handle(request, CancellationToken.None);

        _catalogRepository.Verify(r => r.DeleteCategory(_categoryId), Times.Once);
        _catalogRepository.Verify(r => r.DeleteSkusByCategory(_categoryId), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_CategoryHasSkusButNoneInStockOrDelivery_DeletesCategoryAndCommits()
    {
        var sku1 = MakeSku(_categoryId);
        var sku2 = MakeSku(_categoryId);
        var category = new Category { Id = _categoryId, CategoryName = "market", NormalizedCategoryName = "MARKET", Skus = [sku1, sku2] };
        _catalogRepository.Setup(r => r.GetCategory(_categoryId)).ReturnsAsync(category);
        _stockStateRepository.Setup(r => r.HasStockInAnyWarehouse(It.IsAny<Guid>())).ReturnsAsync(false);
        _deliveryRepository.Setup(r => r.HasIncomingDeliveriesWithSku(It.IsAny<Guid>())).ReturnsAsync(false);

        var request = new DeleteCategoryCommandRequest { CategoryId = _categoryId.ToString() };

        await _handler.Handle(request, CancellationToken.None);

        _stockStateRepository.Verify(r => r.HasStockInAnyWarehouse(sku1.Id), Times.Once);
        _stockStateRepository.Verify(r => r.HasStockInAnyWarehouse(sku2.Id), Times.Once);
        _catalogRepository.Verify(r => r.DeleteCategory(_categoryId), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ASkuHasStock_ThrowsAndDoesNotDelete()
    {
        var sku = MakeSku(_categoryId);
        var category = new Category { Id = _categoryId, CategoryName = "market", NormalizedCategoryName = "MARKET", Skus = [sku] };
        _catalogRepository.Setup(r => r.GetCategory(_categoryId)).ReturnsAsync(category);
        _stockStateRepository.Setup(r => r.HasStockInAnyWarehouse(sku.Id)).ReturnsAsync(true);
        _deliveryRepository.Setup(r => r.HasIncomingDeliveriesWithSku(sku.Id)).ReturnsAsync(false);

        var request = new DeleteCategoryCommandRequest { CategoryId = _categoryId.ToString() };

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("This category cannot be deleted because it contains SKUs that exist in stock or pending deliveries.", exception.Message);
        _catalogRepository.Verify(r => r.DeleteCategory(It.IsAny<Guid>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ASkuHasPendingDelivery_ThrowsAndDoesNotDelete()
    {
        var sku = MakeSku(_categoryId);
        var category = new Category { Id = _categoryId, CategoryName = "market", NormalizedCategoryName = "MARKET", Skus = [sku] };
        _catalogRepository.Setup(r => r.GetCategory(_categoryId)).ReturnsAsync(category);
        _stockStateRepository.Setup(r => r.HasStockInAnyWarehouse(sku.Id)).ReturnsAsync(false);
        _deliveryRepository.Setup(r => r.HasIncomingDeliveriesWithSku(sku.Id)).ReturnsAsync(true);

        var request = new DeleteCategoryCommandRequest { CategoryId = _categoryId.ToString() };

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("This category cannot be deleted because it contains SKUs that exist in stock or pending deliveries.", exception.Message);
        _catalogRepository.Verify(r => r.DeleteCategory(It.IsAny<Guid>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_FirstSkuAlreadyBlocksDeletion_StopsCheckingRemainingSkus()
    {
        var blockedSku = MakeSku(_categoryId);
        var neverCheckedSku = MakeSku(_categoryId);
        var category = new Category
        {
            Id = _categoryId,
            CategoryName = "market",
            NormalizedCategoryName = "MARKET",
            Skus = [blockedSku, neverCheckedSku]
        };
        _catalogRepository.Setup(r => r.GetCategory(_categoryId)).ReturnsAsync(category);
        _stockStateRepository.Setup(r => r.HasStockInAnyWarehouse(blockedSku.Id)).ReturnsAsync(true);
        _deliveryRepository.Setup(r => r.HasIncomingDeliveriesWithSku(blockedSku.Id)).ReturnsAsync(false);

        var request = new DeleteCategoryCommandRequest { CategoryId = _categoryId.ToString() };

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));

        _stockStateRepository.Verify(r => r.HasStockInAnyWarehouse(neverCheckedSku.Id), Times.Never);
        _deliveryRepository.Verify(r => r.HasIncomingDeliveriesWithSku(neverCheckedSku.Id), Times.Never);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsNullReferenceException()
    {
        // Documents a current gap rather than intended behavior: the handler never
        // null-checks the repository result before touching category.Skus, so a
        // missing category surfaces as an unhandled NullReferenceException instead
        // of a domain-level "not found" error.
        _catalogRepository.Setup(r => r.GetCategory(_categoryId)).ReturnsAsync((Category)null!);

        var request = new DeleteCategoryCommandRequest { CategoryId = _categoryId.ToString() };

        await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(request, CancellationToken.None));
    }
}
