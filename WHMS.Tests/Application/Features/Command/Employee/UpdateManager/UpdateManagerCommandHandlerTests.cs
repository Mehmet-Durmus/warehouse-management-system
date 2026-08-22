using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Employee.UpdateManager;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Command.Employee.UpdateManager;

public class UpdateManagerCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepository = new();
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdateManagerCommandHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();

    public UpdateManagerCommandHandlerTests()
    {
        _handler = new UpdateManagerCommandHandler(_employeeRepository.Object, _warehouseRepository.Object, _unitOfWork.Object);
    }

    private ApplicationUser ExistingManager() => new() { Id = _userId, UserName = "WHM_0001", FullName = "Eski Isim" };

    [Fact]
    public async Task Handle_ManagerNotFound_ThrowsAndDoesNotCommit()
    {
        _employeeRepository.Setup(r => r.GetManager(_userId)).ReturnsAsync((ApplicationUser)null!);

        var request = new UpdateManagerCommandRequest { UserId = _userId.ToString(), FullName = "Yeni Isim" };

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Manager not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NoWarehouseIdProvided_UpdatesFullNameOnlyWithoutCheckingWarehouse()
    {
        var manager = ExistingManager();
        _employeeRepository.Setup(r => r.GetManager(_userId)).ReturnsAsync(manager);

        var request = new UpdateManagerCommandRequest { UserId = _userId.ToString(), FullName = "Yeni Isim", WarehouseId = null };

        var response = await _handler.Handle(request, CancellationToken.None);

        _warehouseRepository.Verify(r => r.WarehouseExists(It.IsAny<Guid>()), Times.Never);
        Assert.Equal("Yeni Isim", manager.FullName);
        Assert.Null(manager.WarehouseId);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("Yeni Isim", response.FullName);
    }

    [Fact]
    public async Task Handle_WarehouseIdProvidedButDoesNotExist_ThrowsAndLeavesFullNameUnchanged()
    {
        var manager = ExistingManager();
        _employeeRepository.Setup(r => r.GetManager(_userId)).ReturnsAsync(manager);
        _warehouseRepository.Setup(r => r.WarehouseExists(_warehouseId)).ReturnsAsync(false);

        var request = new UpdateManagerCommandRequest { UserId = _userId.ToString(), FullName = "Yeni Isim", WarehouseId = _warehouseId.ToString() };

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Warehouse not found.", exception.Message);
        Assert.Equal("Eski Isim", manager.FullName);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WarehouseIdProvidedAndExists_UpdatesBothFieldsAndCommits()
    {
        var manager = ExistingManager();
        _employeeRepository.Setup(r => r.GetManager(_userId)).ReturnsAsync(manager);
        _warehouseRepository.Setup(r => r.WarehouseExists(_warehouseId)).ReturnsAsync(true);

        var request = new UpdateManagerCommandRequest { UserId = _userId.ToString(), FullName = "Yeni Isim", WarehouseId = _warehouseId.ToString() };

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal("Yeni Isim", manager.FullName);
        Assert.Equal(_warehouseId, manager.WarehouseId);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal(_warehouseId.ToString(), response.WarehouseId);
    }
}
