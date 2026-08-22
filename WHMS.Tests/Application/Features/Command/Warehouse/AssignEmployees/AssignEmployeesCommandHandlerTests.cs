using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Features.Command.Warehouse.AssignEmployees;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Command.Warehouse.AssignEmployees;

public class AssignEmployeesCommandHandlerTests
{
    private readonly Mock<IAuthService> _authService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly AssignEmployeesCommandHandler _handler;
    private readonly Guid _warehouseId = Guid.NewGuid();

    public AssignEmployeesCommandHandlerTests()
    {
        _handler = new AssignEmployeesCommandHandler(_authService.Object, _unitOfWork.Object);
    }

    private AssignEmployeesCommandRequest Request(string? managerId = null, List<string>? staffIds = null) => new()
    {
        WarehouseId = _warehouseId.ToString(),
        ManagerId = managerId,
        StaffIds = staffIds
    };

    [Fact]
    public async Task Handle_NoManagerOrStaffProvided_ReturnsNoWarningsAndStillCommits()
    {
        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Empty(response.Warnings!);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ManagerIdInvalid_AddsNotFoundWarning()
    {
        _authService.Setup(s => s.FindByIdAsync("missing")).ReturnsAsync((ApplicationUser?)null);

        var response = await _handler.Handle(Request(managerId: "missing"), CancellationToken.None);

        Assert.Equal(["'missing' is invalid. Manager not found."], response.Warnings);
    }

    [Fact]
    public async Task Handle_ManagerIdIsNotAManager_AddsRoleWarning()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "WHS_0001", FullName = "Test Staff" };
        _authService.Setup(s => s.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);
        _authService.Setup(s => s.GetRolesAsync(user)).ReturnsAsync([ApplicationRole.WarehouseStaff]);

        var response = await _handler.Handle(Request(managerId: user.Id.ToString()), CancellationToken.None);

        Assert.Equal(["Test Staff is not a manager."], response.Warnings);
        Assert.Null(user.WarehouseId);
    }

    [Fact]
    public async Task Handle_ManagerAlreadyAssignedElsewhere_AddsWarningAndDoesNotReassign()
    {
        var otherWarehouseId = Guid.NewGuid();
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "WHM_0001", FullName = "Test Manager", WarehouseId = otherWarehouseId };
        _authService.Setup(s => s.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);
        _authService.Setup(s => s.GetRolesAsync(user)).ReturnsAsync([ApplicationRole.WarehouseManager]);

        var response = await _handler.Handle(Request(managerId: user.Id.ToString()), CancellationToken.None);

        Assert.Equal(["Test Manager works at a different warehouse."], response.Warnings);
        Assert.Equal(otherWarehouseId, user.WarehouseId);
    }

    [Fact]
    public async Task Handle_ManagerValidAndUnassigned_AssignsWarehouseWithoutWarning()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "WHM_0001", FullName = "Test Manager" };
        _authService.Setup(s => s.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);
        _authService.Setup(s => s.GetRolesAsync(user)).ReturnsAsync([ApplicationRole.WarehouseManager]);

        var response = await _handler.Handle(Request(managerId: user.Id.ToString()), CancellationToken.None);

        Assert.Empty(response.Warnings!);
        Assert.Equal(_warehouseId, user.WarehouseId);
    }

    [Fact]
    public async Task Handle_MixOfValidAndInvalidStaffIds_ProcessesEachIndependently()
    {
        var validStaff = new ApplicationUser { Id = Guid.NewGuid(), UserName = "WHS_0001", FullName = "Valid Staff" };
        var wrongRoleStaff = new ApplicationUser { Id = Guid.NewGuid(), UserName = "WHM_0002", FullName = "Wrong Role" };
        _authService.Setup(s => s.FindByIdAsync(validStaff.Id.ToString())).ReturnsAsync(validStaff);
        _authService.Setup(s => s.GetRolesAsync(validStaff)).ReturnsAsync([ApplicationRole.WarehouseStaff]);
        _authService.Setup(s => s.FindByIdAsync(wrongRoleStaff.Id.ToString())).ReturnsAsync(wrongRoleStaff);
        _authService.Setup(s => s.GetRolesAsync(wrongRoleStaff)).ReturnsAsync([ApplicationRole.WarehouseManager]);
        _authService.Setup(s => s.FindByIdAsync("missing")).ReturnsAsync((ApplicationUser?)null);

        var response = await _handler.Handle(
            Request(staffIds: [validStaff.Id.ToString(), wrongRoleStaff.Id.ToString(), "missing"]), CancellationToken.None);

        Assert.Equal(_warehouseId, validStaff.WarehouseId);
        Assert.Null(wrongRoleStaff.WarehouseId);
        Assert.Equal(
        [
            "Wrong Role is not a staff member.",
            "'missing' is invalid. Staff member not found."
        ], response.Warnings);
    }
}
