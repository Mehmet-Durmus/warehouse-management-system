using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Employee.UpdateStaffMember;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Employee.UpdateStaffMember;

public class UpdateStaffMemberCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepository = new();
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdateStaffMemberCommandHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();

    public UpdateStaffMemberCommandHandlerTests()
    {
        _handler = new UpdateStaffMemberCommandHandler(_employeeRepository.Object, _warehouseRepository.Object, _unitOfWork.Object);
    }

    private ApplicationUser ExistingStaff() => new() { Id = _userId, UserName = "WHS_0001", FullName = "Eski Isim" };

    [Fact]
    public async Task Handle_StaffNotFound_ThrowsAndDoesNotCommit()
    {
        _employeeRepository.Setup(r => r.GetStaffMember(_userId)).ReturnsAsync((ApplicationUser)null!);

        var request = new UpdateStaffMemberCommandRequest { UserId = _userId.ToString(), FullName = "Yeni Isim" };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Staff member not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NoWarehouseIdProvided_UpdatesFullNameOnlyWithoutCheckingWarehouse()
    {
        var staff = ExistingStaff();
        _employeeRepository.Setup(r => r.GetStaffMember(_userId)).ReturnsAsync(staff);

        var request = new UpdateStaffMemberCommandRequest { UserId = _userId.ToString(), FullName = "Yeni Isim", WarehouseId = null };

        var response = await _handler.Handle(request, CancellationToken.None);

        _warehouseRepository.Verify(r => r.WarehouseExists(It.IsAny<Guid>()), Times.Never);
        Assert.Equal("Yeni Isim", staff.FullName);
        Assert.Null(staff.WarehouseId);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("Yeni Isim", response.FullName);
    }

    [Fact]
    public async Task Handle_WarehouseIdProvidedButDoesNotExist_ThrowsAndLeavesFullNameUnchanged()
    {
        var staff = ExistingStaff();
        _employeeRepository.Setup(r => r.GetStaffMember(_userId)).ReturnsAsync(staff);
        _warehouseRepository.Setup(r => r.WarehouseExists(_warehouseId)).ReturnsAsync(false);

        var request = new UpdateStaffMemberCommandRequest { UserId = _userId.ToString(), FullName = "Yeni Isim", WarehouseId = _warehouseId.ToString() };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Warehouse not found.", exception.Message);
        Assert.Equal("Eski Isim", staff.FullName);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WarehouseIdProvidedAndExists_UpdatesBothFieldsAndCommits()
    {
        var staff = ExistingStaff();
        _employeeRepository.Setup(r => r.GetStaffMember(_userId)).ReturnsAsync(staff);
        _warehouseRepository.Setup(r => r.WarehouseExists(_warehouseId)).ReturnsAsync(true);

        var request = new UpdateStaffMemberCommandRequest { UserId = _userId.ToString(), FullName = "Yeni Isim", WarehouseId = _warehouseId.ToString() };

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal("Yeni Isim", staff.FullName);
        Assert.Equal(_warehouseId, staff.WarehouseId);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal(_warehouseId.ToString(), response.WarehouseId);
    }
}
