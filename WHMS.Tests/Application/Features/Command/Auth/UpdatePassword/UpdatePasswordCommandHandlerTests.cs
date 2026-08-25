using Microsoft.AspNetCore.Identity;
using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Auth.UpdatePassword;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Command.Auth.UpdatePassword;

public class UpdatePasswordCommandHandlerTests
{
    private readonly Mock<IAuthService> _authService = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdatePasswordCommandHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();

    public UpdatePasswordCommandHandlerTests()
    {
        _currentUserService.Setup(u => u.UserId).Returns(_userId);
        _handler = new UpdatePasswordCommandHandler(_authService.Object, _currentUserService.Object, _unitOfWork.Object);
    }

    private UpdatePasswordCommandRequest Request() => new()
    {
        CurrentPassword = "OldPass1",
        Password = "NewPass1",
        PasswordConfirm = "NewPass1"
    };

    [Fact]
    public async Task Handle_ChangeSucceeds_MarksPasswordChangedAndCommits()
    {
        var user = new ApplicationUser { Id = _userId, UserName = "WHD_0002", FullName = "Test User", IsPasswordChanged = false };
        _authService.Setup(s => s.FindByIdAsync(_userId.ToString())).ReturnsAsync(user);
        _authService.Setup(s => s.ChangePasswordAsync(user, "OldPass1", "NewPass1")).ReturnsAsync(IdentityResult.Success);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.True(user.IsPasswordChanged);
        Assert.True(user.PasswordChangedAt > DateTime.MinValue);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Null(response.Errors);
    }

    [Fact]
    public async Task Handle_ChangeFails_ReturnsErrorsWithoutMutatingUserOrCommitting()
    {
        var user = new ApplicationUser { Id = _userId, UserName = "WHD_0002", FullName = "Test User", IsPasswordChanged = false };
        _authService.Setup(s => s.FindByIdAsync(_userId.ToString())).ReturnsAsync(user);
        _authService
            .Setup(s => s.ChangePasswordAsync(user, "OldPass1", "NewPass1"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Incorrect password." }));

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.False(user.IsPasswordChanged);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        Assert.Equal(["Incorrect password."], response.Errors);
    }
}
