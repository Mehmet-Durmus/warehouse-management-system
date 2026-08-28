using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Catalog.CreateSku;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Catalog.CreateSku;

public class CreateSkuCommandHandlerTests
{
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreateSkuCommandHandler _handler;
    private readonly Guid _categoryId = Guid.NewGuid();

    public CreateSkuCommandHandlerTests()
    {
        _handler = new CreateSkuCommandHandler(_catalogRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_NameIsUniqueAndCategoryExists_PersistsSkuAndReturnsResponse()
    {
        var category = new Category { Id = _categoryId, CategoryName = "market", NormalizedCategoryName = "MARKET" };
        _catalogRepository.Setup(r => r.IsSkuNameExists("KOLA")).ReturnsAsync(false);
        _catalogRepository.Setup(r => r.GetCategory(_categoryId)).ReturnsAsync(category);

        var request = new CreateSkuCommandRequest
        {
            SkuName = "kola",
            Barcode = "8690000000001",
            UnitPrice = 25.50m,
            CategoryId = _categoryId.ToString()
        };

        var response = await _handler.Handle(request, CancellationToken.None);

        _catalogRepository.Verify(r => r.AddSku(It.Is<SKU>(s =>
            s.SKUName == "kola" &&
            s.NormalizedSKUName == "KOLA" &&
            s.Barcode == "8690000000001" &&
            s.UnitPrice == 25.50m &&
            s.CategoryId == _categoryId)), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);

        Assert.Equal("kola", response.SkuName);
        Assert.Equal(_categoryId.ToString(), response.CategoryId);
        Assert.Equal("8690000000001", response.Barcode);
        Assert.Equal(25.50m, response.UnitPrice);
    }

    [Fact]
    public async Task Handle_SkuNameAlreadyExists_ThrowsWithoutCheckingCategory()
    {
        _catalogRepository.Setup(r => r.IsSkuNameExists("KOLA")).ReturnsAsync(true);

        var request = new CreateSkuCommandRequest
        {
            SkuName = "kola",
            Barcode = "8690000000001",
            UnitPrice = 25.50m,
            CategoryId = _categoryId.ToString()
        };

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Sku already exists.", exception.Message);
        _catalogRepository.Verify(r => r.GetCategory(It.IsAny<Guid>()), Times.Never);
        _catalogRepository.Verify(r => r.AddSku(It.IsAny<SKU>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsAndDoesNotPersist()
    {
        _catalogRepository.Setup(r => r.IsSkuNameExists("KOLA")).ReturnsAsync(false);
        _catalogRepository.Setup(r => r.GetCategory(_categoryId)).ReturnsAsync((Category)null!);

        var request = new CreateSkuCommandRequest
        {
            SkuName = "kola",
            Barcode = "8690000000001",
            UnitPrice = 25.50m,
            CategoryId = _categoryId.ToString()
        };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Category not found.", exception.Message);
        _catalogRepository.Verify(r => r.AddSku(It.IsAny<SKU>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }
}
