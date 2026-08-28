using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Catalog.CreateCategory;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Catalog.CreateCategory;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _handler = new CreateCategoryCommandHandler(_catalogRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_CategoryNameIsUnique_PersistsCategoryAndReturnsResponse()
    {
        _catalogRepository
            .Setup(r => r.IsCategoryNameExists("OYUNCAK"))
            .ReturnsAsync(false);

        var request = new CreateCategoryCommandRequest { CategoryName = "oyuncak" };

        var response = await _handler.Handle(request, CancellationToken.None);

        _catalogRepository.Verify(r => r.AddCategory(It.Is<Category>(c =>
            c.CategoryName == "oyuncak" && c.NormalizedCategoryName == "OYUNCAK")), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("oyuncak", response.CategoryName);
    }

    [Fact]
    public async Task Handle_CategoryNameHasTurkishCharacters_NormalizesUsingTurkishCulture()
    {
        _catalogRepository
            .Setup(r => r.IsCategoryNameExists("KIRTASİYE"))
            .ReturnsAsync(false);

        var request = new CreateCategoryCommandRequest { CategoryName = "kırtasiye" };

        await _handler.Handle(request, CancellationToken.None);

        _catalogRepository.Verify(r => r.AddCategory(It.Is<Category>(c =>
            c.NormalizedCategoryName == "KIRTASİYE")), Times.Once);
    }

    [Fact]
    public async Task Handle_CategoryNameAlreadyExists_ThrowsAndDoesNotPersist()
    {
        _catalogRepository
            .Setup(r => r.IsCategoryNameExists("OYUNCAK"))
            .ReturnsAsync(true);

        var request = new CreateCategoryCommandRequest { CategoryName = "oyuncak" };

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Category already exists.", exception.Message);
        _catalogRepository.Verify(r => r.AddCategory(It.IsAny<Category>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }
}
