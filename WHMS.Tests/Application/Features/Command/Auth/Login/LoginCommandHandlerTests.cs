using Microsoft.AspNetCore.Identity;
using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Configurations;
using WHMS.Application.Common.DTOs;
using WHMS.Application.Features.Command.Auth.Login;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Command.Auth.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<IAuthService> _authService = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly JwtSettings _jwtSettings = new() { Key = "unit-test-signing-key-unit-test", Issuer = "whms", Audience = "whms", DurationInMinutes = 60 };
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_tokenService.Object, _jwtSettings, _authService.Object);
    }

    private static ApplicationUser MakeUser() => new() { Id = Guid.NewGuid(), UserName = "WHD_0001", FullName = "Test User" };

    [Fact]
    public async Task Handle_UserNotFound_ThrowsInvalidCredentials()
    {
        _authService.Setup(s => s.FindByNameAsync("unknown")).ReturnsAsync((ApplicationUser?)null);

        var request = new LoginCommandRequest { UserName = "unknown", Password = "whatever" };

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));

        // Note: "credantials" is a typo already present in the source; preserved as-is.
        Assert.Equal("Invalid credantials!", exception.Message);
        _tokenService.Verify(t => t.CreateAccessToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PasswordIncorrect_ThrowsInvalidCredentialsWithoutRevealingWhichCheckFailed()
    {
        var user = MakeUser();
        _authService.Setup(s => s.FindByNameAsync("WHD_0001")).ReturnsAsync(user);
        _authService.Setup(s => s.CheckPasswordSignInAsync(user, "wrong", false)).ReturnsAsync(SignInResult.Failed);

        var request = new LoginCommandRequest { UserName = "WHD_0001", Password = "wrong" };

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Invalid credantials!", exception.Message);
        _tokenService.Verify(t => t.CreateAccessToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CredentialsValid_ReturnsAccessTokenFromTokenService()
    {
        var user = MakeUser();
        var roles = new List<string> { "WarehouseStaff" };
        var expiration = new DateTime(2026, 1, 1);
        _authService.Setup(s => s.FindByNameAsync("WHD_0001")).ReturnsAsync(user);
        _authService.Setup(s => s.CheckPasswordSignInAsync(user, "correct", false)).ReturnsAsync(SignInResult.Success);
        _authService.Setup(s => s.GetRolesAsync(user)).ReturnsAsync(roles);
        _tokenService
            .Setup(t => t.CreateAccessToken(user, roles, _jwtSettings.DurationInMinutes))
            .Returns(new AccessTokenDto { AccessToken = "signed-token", Expiration = expiration });

        var request = new LoginCommandRequest { UserName = "WHD_0001", Password = "correct" };

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal("signed-token", response.AccessToken);
        Assert.Equal(expiration, response.Expiration);
    }
}
