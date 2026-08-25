using Microsoft.AspNetCore.Identity;
using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Employee.UpdateEmployeePassword;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Command.Employee.UpdateEmployeePassword;

public class UpdateEmployeePasswordCommandHandlerTests
{
    private readonly Mock<IAuthService> _authService = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<IPasswordCreator> _passwordCreator = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdateEmployeePasswordCommandHandler _handler;
    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Guid _employeeId = Guid.NewGuid();

    public UpdateEmployeePasswordCommandHandlerTests()
    {
        _currentUserService.Setup(u => u.UserId).Returns(_currentUserId);
        _handler = new UpdateEmployeePasswordCommandHandler(_authService.Object, _currentUserService.Object, _passwordCreator.Object, _unitOfWork.Object);
    }

    private UpdateEmployeePasswordCommandRequest Request() => new() { EmployeeId = _employeeId.ToString() };

    [Fact]
    public async Task Handle_EmployeeNotFound_ThrowsAndDoesNotCommit()
    {
        _authService.Setup(s => s.FindByIdAsync(_employeeId.ToString())).ReturnsAsync((ApplicationUser?)null);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Employee not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_TargetIsTheCurrentUser_ThrowsSameNotFoundMessage()
    {
        var self = new ApplicationUser { Id = _currentUserId, UserName = "WHD_0001", FullName = "Self" };
        _authService.Setup(s => s.FindByIdAsync(_currentUserId.ToString())).ReturnsAsync(self);

        var request = new UpdateEmployeePasswordCommandRequest { EmployeeId = _currentUserId.ToString() };

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Employee not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ResetSucceeds_MarksPasswordChangedAndReturnsTempPassword()
    {
        var employee = new ApplicationUser { Id = _employeeId, UserName = "WHS_0001", FullName = "Test Staff", IsPasswordChanged = true };
        _authService.Setup(s => s.FindByIdAsync(_employeeId.ToString())).ReturnsAsync(employee);
        _passwordCreator.Setup(p => p.CreateTempPassword()).ReturnsAsync("Ab3xYz");
        _authService.Setup(s => s.GeneratePasswordResetTokenAsync(employee)).ReturnsAsync("reset-token");
        _authService.Setup(s => s.ResetPasswordAsync(employee, "reset-token", "Ab3xYz")).ReturnsAsync(IdentityResult.Success);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.False(employee.IsPasswordChanged);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.NotNull(response.Result);
        Assert.Equal(_employeeId.ToString(), response.Result!.EmployeeId);
        Assert.Equal("WHS_0001", response.Result.EmployeeUserName);
        Assert.Equal("Ab3xYz", response.Result.TempPassword);
        Assert.Null(response.Errors);
    }

    [Fact]
    public async Task Handle_ResetFails_ReturnsErrorsWithoutMutatingOrCommitting()
    {
        var employee = new ApplicationUser { Id = _employeeId, UserName = "WHS_0001", FullName = "Test Staff", IsPasswordChanged = true };
        _authService.Setup(s => s.FindByIdAsync(_employeeId.ToString())).ReturnsAsync(employee);
        _passwordCreator.Setup(p => p.CreateTempPassword()).ReturnsAsync("Ab3xYz");
        _authService.Setup(s => s.GeneratePasswordResetTokenAsync(employee)).ReturnsAsync("reset-token");
        _authService
            .Setup(s => s.ResetPasswordAsync(employee, "reset-token", "Ab3xYz"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak." }));

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.True(employee.IsPasswordChanged);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        Assert.Null(response.Result);
        Assert.Equal(["Password too weak."], response.Errors);
    }
}
