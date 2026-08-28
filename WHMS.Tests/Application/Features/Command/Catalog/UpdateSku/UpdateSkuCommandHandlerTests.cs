using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Catalog.UpdateSku;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Catalog.UpdateSku;

public class UpdateSkuCommandHandlerTests
{
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdateSkuCommandHandler _handler;
    private readonly Guid _skuId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();

    public UpdateSkuCommandHandlerTests()
    {
        _handler = new UpdateSkuCommandHandler(_catalogRepository.Object, _unitOfWork.Object);
    }

    private SKU ExistingSku() => new()
    {
        Id = _skuId,
        SKUName = "kola",
        NormalizedSKUName = "KOLA",
        Barcode = "8690000000001",
        UnitPrice = 20m,
        CategoryId = _categoryId
    };

    [Fact]
    public async Task Handle_SkuNotFound_ThrowsAndDoesNotCommit()
    {
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync((SKU)null!);

        var request = new UpdateSkuCommandRequest { SkuId = _skuId.ToString(), SkuName = "kola", CategoryId = _categoryId.ToString() };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Sku not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NameChangedAndUniqueAndCategoryExists_UpdatesSkuAndCommits()
    {
        var sku = ExistingSku();
        var category = new Category { Id = _categoryId, CategoryName = "market", NormalizedCategoryName = "MARKET" };
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync(sku);
        _catalogRepository.Setup(r => r.IsSkuNameExists("MEYVE SUYU")).ReturnsAsync(false);
        _catalogRepository.Setup(r => r.GetCategory(_categoryId)).ReturnsAsync(category);

        var request = new UpdateSkuCommandRequest
        {
            SkuId = _skuId.ToString(),
            SkuName = "meyve suyu",
            Barcode = "8690000000002",
            UnitPrice = 30m,
            CategoryId = _categoryId.ToString()
        };

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal("meyve suyu", sku.SKUName);
        Assert.Equal("MEYVE SUYU", sku.NormalizedSKUName);
        Assert.Equal("8690000000002", sku.Barcode);
        Assert.Equal(30m, sku.UnitPrice);
        _catalogRepository.Verify(r => r.UpdateSku(sku), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("meyve suyu", response.SkuName);
    }

    [Fact]
    public async Task Handle_NameChangedButAlreadyExists_ThrowsWithoutCheckingCategory()
    {
        var sku = ExistingSku();
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync(sku);
        _catalogRepository.Setup(r => r.IsSkuNameExists("MEYVE SUYU")).ReturnsAsync(true);

        var request = new UpdateSkuCommandRequest
        {
            SkuId = _skuId.ToString(),
            SkuName = "meyve suyu",
            Barcode = "8690000000002",
            UnitPrice = 30m,
            CategoryId = _categoryId.ToString()
        };

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Sku already exists.", exception.Message);
        _catalogRepository.Verify(r => r.GetCategory(It.IsAny<Guid>()), Times.Never);
        _catalogRepository.Verify(r => r.UpdateSku(It.IsAny<SKU>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NormalizedNameUnchanged_SkipsUniquenessCheckButStillLooksUpCategory()
    {
        var sku = ExistingSku();
        var category = new Category { Id = _categoryId, CategoryName = "market", NormalizedCategoryName = "MARKET" };
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync(sku);
        _catalogRepository.Setup(r => r.GetCategory(_categoryId)).ReturnsAsync(category);

        var request = new UpdateSkuCommandRequest
        {
            SkuId = _skuId.ToString(),
            SkuName = "kola",
            Barcode = "8690000000001",
            UnitPrice = 22m,
            CategoryId = _categoryId.ToString()
        };

        await _handler.Handle(request, CancellationToken.None);

        _catalogRepository.Verify(r => r.IsSkuNameExists(It.IsAny<string>()), Times.Never);
        _catalogRepository.Verify(r => r.GetCategory(_categoryId), Times.Once);
        Assert.Equal(22m, sku.UnitPrice);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsAndLeavesSkuUnchanged()
    {
        var sku = ExistingSku();
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync(sku);
        _catalogRepository.Setup(r => r.IsSkuNameExists("MEYVE SUYU")).ReturnsAsync(false);
        _catalogRepository.Setup(r => r.GetCategory(_categoryId)).ReturnsAsync((Category)null!);

        var request = new UpdateSkuCommandRequest
        {
            SkuId = _skuId.ToString(),
            SkuName = "meyve suyu",
            Barcode = "8690000000002",
            UnitPrice = 30m,
            CategoryId = _categoryId.ToString()
        };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));

        // Now routed through CategoryRules.EnsureExists, so the message matches
        // every other "Category not found." guard - the previous inconsistency
        // (missing trailing period) is gone as a side effect of this migration.
        Assert.Equal("Category not found.", exception.Message);
        Assert.Equal("kola", sku.SKUName);
        _catalogRepository.Verify(r => r.UpdateSku(It.IsAny<SKU>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }
}
