using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.InventoryCount.CreateInventoryCount;

namespace WHMS.Tests.Application.Features.Command.InventoryCount.CreateInventoryCount;

public class CreateInventoryCountCommandHandlerTests
{
    private readonly Mock<IInventoryCountRepository> _inventoryCountRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly CreateInventoryCountCommandHandler _handler;
    private readonly Guid _warehouseId = Guid.NewGuid();

    public CreateInventoryCountCommandHandlerTests()
    {
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        _handler = new CreateInventoryCountCommandHandler(_inventoryCountRepository.Object, _unitOfWork.Object, _currentUserService.Object);
    }

    [Fact]
    public async Task Handle_UncompletedInventoryCountAlreadyExists_ThrowsAndDoesNotCreate()
    {
        _inventoryCountRepository.Setup(r => r.IsThereUncompletedInventoryCount(_warehouseId)).ReturnsAsync(true);

        // Note: "uncomplated" is a typo already present in the source; preserved as-is.
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(new(), CancellationToken.None));

        Assert.Equal("There is uncomplated inventory count.", exception.Message);
        _inventoryCountRepository.Verify(r => r.CreateInventoryCount(It.IsAny<WHMS.Domain.Entities.InventoryCount>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NoUncompletedInventoryCount_CreatesAndReturnsId()
    {
        _inventoryCountRepository.Setup(r => r.IsThereUncompletedInventoryCount(_warehouseId)).ReturnsAsync(false);

        var response = await _handler.Handle(new(), CancellationToken.None);

        _inventoryCountRepository.Verify(r => r.CreateInventoryCount(It.Is<WHMS.Domain.Entities.InventoryCount>(ic =>
            ic.WarehouseId == _warehouseId && ic.IsCompleted == false)), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.False(string.IsNullOrEmpty(response.InventoryCountId));
    }
}
