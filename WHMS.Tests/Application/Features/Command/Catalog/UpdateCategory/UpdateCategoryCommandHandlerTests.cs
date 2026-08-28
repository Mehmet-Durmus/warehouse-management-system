using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Catalog.UpdateCategory;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Catalog.UpdateCategory;

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdateCategoryCommandHandler _handler;
    private readonly Guid _categoryId = Guid.NewGuid();

    public UpdateCategoryCommandHandlerTests()
    {
        _handler = new UpdateCategoryCommandHandler(_catalogRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsAndDoesNotCommit()
    {
        _catalogRepository.Setup(r => r.GetCategory(_categoryId)).ReturnsAsync((Category)null!);

        var request = new UpdateCategoryCommandRequest { CategoryId = _categoryId.ToString(), CategoryName = "oyuncak" };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Category not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NewNameIsUniqueAndDifferent_UpdatesCategoryAndCommits()
    {
        var category = new Category { Id = _categoryId, CategoryName = "oyuncak", NormalizedCategoryName = "OYUNCAK" };
        _catalogRepository.Setup(r => r.GetCategory(_categoryId)).ReturnsAsync(category);
        _catalogRepository.Setup(r => r.IsCategoryNameExists("MARKET")).ReturnsAsync(false);

        var request = new UpdateCategoryCommandRequest { CategoryId = _categoryId.ToString(), CategoryName = "market" };

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal("market", category.CategoryName);
        Assert.Equal("MARKET", category.NormalizedCategoryName);
        Assert.Equal("market", response.CategoryName);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_NewNameIsDifferentButAlreadyExists_ThrowsAndLeavesCategoryUnchanged()
    {
        var category = new Category { Id = _categoryId, CategoryName = "oyuncak", NormalizedCategoryName = "OYUNCAK" };
        _catalogRepository.Setup(r => r.GetCategory(_categoryId)).ReturnsAsync(category);
        _catalogRepository.Setup(r => r.IsCategoryNameExists("MARKET")).ReturnsAsync(true);

        var request = new UpdateCategoryCommandRequest { CategoryId = _categoryId.ToString(), CategoryName = "market" };

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Category already exists.", exception.Message);
        Assert.Equal("oyuncak", category.CategoryName);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NormalizedNameUnchanged_SkipsUniquenessCheckAndUpdatesRawCasing()
    {
        var category = new Category { Id = _categoryId, CategoryName = "Oyuncak", NormalizedCategoryName = "OYUNCAK" };
        _catalogRepository.Setup(r => r.GetCategory(_categoryId)).ReturnsAsync(category);

        var request = new UpdateCategoryCommandRequest { CategoryId = _categoryId.ToString(), CategoryName = "oyuncak" };

        var response = await _handler.Handle(request, CancellationToken.None);

        _catalogRepository.Verify(r => r.IsCategoryNameExists(It.IsAny<string>()), Times.Never);
        Assert.Equal("oyuncak", category.CategoryName);
        Assert.Equal("oyuncak", response.CategoryName);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
