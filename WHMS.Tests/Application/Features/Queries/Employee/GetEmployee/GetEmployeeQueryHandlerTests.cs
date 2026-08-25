using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Features.Queries.Employee.GetEmployee;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Queries.Employee.GetEmployee;

public class GetEmployeeQueryHandlerTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly GetEmployeeQueryHandler _handler;
    private readonly Guid _employeeId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();

    public GetEmployeeQueryHandlerTests()
    {
        _handler = new GetEmployeeQueryHandler(_employeeRepository.Object, _currentUserService.Object);
    }

    private void SetCurrentUser(string role) => _currentUserService.Setup(u => u.Roles).Returns([role]);
    private GetEmployeeQueryRequest Request() => new() { EmployeeId = _employeeId.ToString() };

    [Fact]
    public async Task Handle_EmployeeNotFound_Throws()
    {
        SetCurrentUser(ApplicationRole.LogisticDirector);
        _employeeRepository.Setup(r => r.GetEmployee(_employeeId)).ReturnsAsync((ApplicationUser)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Employee not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_ManagerViewingAnotherManager_ThrowsBecauseTargetIsNotStaff()
    {
        SetCurrentUser(ApplicationRole.WarehouseManager);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var target = new ApplicationUser { Id = _employeeId, UserName = "WHM_0002", FullName = "Other Manager", WarehouseId = _warehouseId };
        _employeeRepository.Setup(r => r.GetEmployee(_employeeId)).ReturnsAsync(target);
        _employeeRepository.Setup(r => r.GetEmployeeRole(_employeeId)).ReturnsAsync(ApplicationRole.WarehouseManager);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Employee not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_ManagerViewingStaffFromAnotherWarehouse_Throws()
    {
        SetCurrentUser(ApplicationRole.WarehouseManager);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var target = new ApplicationUser { Id = _employeeId, UserName = "WHS_0001", FullName = "Staff", WarehouseId = Guid.NewGuid() };
        _employeeRepository.Setup(r => r.GetEmployee(_employeeId)).ReturnsAsync(target);
        _employeeRepository.Setup(r => r.GetEmployeeRole(_employeeId)).ReturnsAsync(ApplicationRole.WarehouseStaff);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Employee not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_ManagerViewingOwnWarehouseStaff_ReturnsResponse()
    {
        SetCurrentUser(ApplicationRole.WarehouseManager);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var target = new ApplicationUser
        {
            Id = _employeeId, UserName = "WHS_0001", FullName = "Staff", WarehouseId = _warehouseId,
            Warehouse = new WHMS.Domain.Entities.Warehouse { WarehouseName = "Depo", Address = new WHMS.Domain.ValueObjects.Address(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "34000", "Cadde") }
        };
        _employeeRepository.Setup(r => r.GetEmployee(_employeeId)).ReturnsAsync(target);
        _employeeRepository.Setup(r => r.GetEmployeeRole(_employeeId)).ReturnsAsync(ApplicationRole.WarehouseStaff);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Equal("Staff", response.FullName);
        Assert.Equal(ApplicationRole.WarehouseStaff, response.Role);
    }

    [Fact]
    public async Task Handle_LogisticDirector_CanViewAnyEmployeeRegardlessOfRoleOrWarehouse()
    {
        SetCurrentUser(ApplicationRole.LogisticDirector);
        var target = new ApplicationUser { Id = _employeeId, UserName = "WHM_0001", FullName = "Some Manager", WarehouseId = Guid.NewGuid() };
        _employeeRepository.Setup(r => r.GetEmployee(_employeeId)).ReturnsAsync(target);
        _employeeRepository.Setup(r => r.GetEmployeeRole(_employeeId)).ReturnsAsync(ApplicationRole.WarehouseManager);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Equal("Some Manager", response.FullName);
    }
}
