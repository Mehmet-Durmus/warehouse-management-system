using Microsoft.AspNetCore.Identity;
using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Features.Command.Employee.CreateStaffMember;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Employee.CreateStaffMember;

public class CreateStaffMemberCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepository = new();
    private readonly Mock<IAuthService> _authService = new();
    private readonly Mock<IPasswordCreator> _passwordCreator = new();
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();
    private readonly CreateStaffMemberCommandHandler _handler;
    private readonly Guid _warehouseId = Guid.NewGuid();

    public CreateStaffMemberCommandHandlerTests()
    {
        _employeeRepository.Setup(r => r.GenerateWarehouseStaffUserName()).ReturnsAsync("WHS_0001");
        _passwordCreator.Setup(p => p.CreateTempPassword()).ReturnsAsync("Ab3xYz");
        _handler = new CreateStaffMemberCommandHandler(_employeeRepository.Object, _authService.Object, _passwordCreator.Object, _warehouseRepository.Object);
    }

    [Fact]
    public async Task Handle_NoWarehouseId_CreatesStaffWithoutCheckingWarehouse()
    {
        _authService.Setup(s => s.CreateAsync(It.IsAny<ApplicationUser>(), "Ab3xYz")).ReturnsAsync(IdentityResult.Success);

        var request = new CreateStaffMemberCommandRequest { FullName = "Test Staff", WarehouseId = null };

        var response = await _handler.Handle(request, CancellationToken.None);

        _warehouseRepository.Verify(r => r.WarehouseExists(It.IsAny<Guid>()), Times.Never);
        _authService.Verify(s => s.AddToRoleAsync(It.IsAny<ApplicationUser>(), ApplicationRole.WarehouseStaff), Times.Once);
        Assert.Equal("WHS_0001", response.UserName);
        Assert.Null(response.WarehouseId);
        Assert.Equal("Ab3xYz", response.TempPassword);
    }

    [Fact]
    public async Task Handle_WarehouseIdDoesNotExist_ThrowsWithoutCreatingUser()
    {
        _warehouseRepository.Setup(r => r.WarehouseExists(_warehouseId)).ReturnsAsync(false);

        var request = new CreateStaffMemberCommandRequest { FullName = "Test Staff", WarehouseId = _warehouseId.ToString() };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));

        // Now routed through WarehouseRules.EnsureExists, so the message no longer
        // has the "Watehouse" typo that was previously here - a deliberate cleanup.
        Assert.Equal("Warehouse not found.", exception.Message);
        _authService.Verify(s => s.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WarehouseIdExists_CreatesStaffWithWarehouseAssigned()
    {
        _warehouseRepository.Setup(r => r.WarehouseExists(_warehouseId)).ReturnsAsync(true);
        _authService.Setup(s => s.CreateAsync(It.IsAny<ApplicationUser>(), "Ab3xYz")).ReturnsAsync(IdentityResult.Success);

        var request = new CreateStaffMemberCommandRequest { FullName = "Test Staff", WarehouseId = _warehouseId.ToString() };

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(_warehouseId.ToString(), response.WarehouseId);
        _authService.Verify(s => s.AddToRoleAsync(It.IsAny<ApplicationUser>(), ApplicationRole.WarehouseStaff), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateAsyncFails_ThrowsAndDoesNotAssignRole()
    {
        _authService.Setup(s => s.CreateAsync(It.IsAny<ApplicationUser>(), "Ab3xYz")).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "weak password" }));

        var request = new CreateStaffMemberCommandRequest { FullName = "Test Staff" };

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Staff member could not be created.", exception.Message);
        _authService.Verify(s => s.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }
}
