using Microsoft.AspNetCore.Identity;
using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Features.Command.Employee.CreateEmployee;
using WHMS.Application.Features.Command.Employee.CreateManager;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Employee.CreateManager;

public class CreateManagerCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepository = new();
    private readonly Mock<IAuthService> _authService = new();
    private readonly Mock<IPasswordCreator> _passwordCreator = new();
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();
    private readonly CreateManagerCommandHandler _handler;
    private readonly Guid _warehouseId = Guid.NewGuid();

    public CreateManagerCommandHandlerTests()
    {
        _employeeRepository.Setup(r => r.GenerateWarehouseManagerUserName()).ReturnsAsync("WHM_0001");
        _passwordCreator.Setup(p => p.CreateTempPassword()).ReturnsAsync("Ab3xYz");
        _handler = new CreateManagerCommandHandler(_employeeRepository.Object, _authService.Object, _passwordCreator.Object, _warehouseRepository.Object);
    }

    [Fact]
    public async Task Handle_NoWarehouseId_CreatesManagerWithoutCheckingWarehouse()
    {
        _authService.Setup(s => s.CreateAsync(It.IsAny<ApplicationUser>(), "Ab3xYz")).ReturnsAsync(IdentityResult.Success);

        var request = new CreateManagerCommandRequest { FullName = "Test Manager", WarehouseId = null };

        var response = await _handler.Handle(request, CancellationToken.None);

        _warehouseRepository.Verify(r => r.WarehouseExists(It.IsAny<Guid>()), Times.Never);
        _employeeRepository.Verify(r => r.HasWarehouseAnyManager(It.IsAny<Guid>()), Times.Never);
        _authService.Verify(s => s.AddToRoleAsync(It.IsAny<ApplicationUser>(), ApplicationRole.WarehouseManager), Times.Once);
        Assert.Equal("WHM_0001", response.UserName);
        Assert.Null(response.WarehouseId);
    }

    [Fact]
    public async Task Handle_WarehouseIdDoesNotExist_ThrowsWithoutCreatingUser()
    {
        _warehouseRepository.Setup(r => r.WarehouseExists(_warehouseId)).ReturnsAsync(false);

        var request = new CreateManagerCommandRequest { FullName = "Test Manager", WarehouseId = _warehouseId.ToString() };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Warehouse not found.", exception.Message);
        _employeeRepository.Verify(r => r.HasWarehouseAnyManager(It.IsAny<Guid>()), Times.Never);
        _authService.Verify(s => s.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WarehouseAlreadyHasAManager_ThrowsWithoutCreatingUser()
    {
        _warehouseRepository.Setup(r => r.WarehouseExists(_warehouseId)).ReturnsAsync(true);
        _employeeRepository.Setup(r => r.HasWarehouseAnyManager(_warehouseId)).ReturnsAsync(true);

        var request = new CreateManagerCommandRequest { FullName = "Test Manager", WarehouseId = _warehouseId.ToString() };

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("The warehouse has already a manager.", exception.Message);
        _authService.Verify(s => s.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WarehouseExistsAndHasNoManager_CreatesManagerWithWarehouseAssigned()
    {
        _warehouseRepository.Setup(r => r.WarehouseExists(_warehouseId)).ReturnsAsync(true);
        _employeeRepository.Setup(r => r.HasWarehouseAnyManager(_warehouseId)).ReturnsAsync(false);
        _authService.Setup(s => s.CreateAsync(It.IsAny<ApplicationUser>(), "Ab3xYz")).ReturnsAsync(IdentityResult.Success);

        var request = new CreateManagerCommandRequest { FullName = "Test Manager", WarehouseId = _warehouseId.ToString() };

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(_warehouseId.ToString(), response.WarehouseId);
        _authService.Verify(s => s.AddToRoleAsync(It.IsAny<ApplicationUser>(), ApplicationRole.WarehouseManager), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateAsyncFails_ThrowsAndDoesNotAssignRole()
    {
        _authService.Setup(s => s.CreateAsync(It.IsAny<ApplicationUser>(), "Ab3xYz")).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "weak password" }));

        var request = new CreateManagerCommandRequest { FullName = "Test Manager" };

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Manager could not be created.", exception.Message);
        _authService.Verify(s => s.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }
}
