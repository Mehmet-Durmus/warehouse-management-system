using Microsoft.AspNetCore.Identity;
using Moq;
using WHMS.Application.Asbstractions.Infrastructure;
using WHMS.Application.Asbstractions.Persistence;
using WHMS.Application.Configurations;
using WHMS.Application.Features.Command.Login;
using WHMS.Domain.Entities;

namespace WHMS.Tests.UnitTests.Application.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly JwtSettings _jwtSettings;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _jwtSettings = new JwtSettings 
        {
            Key = "",
            Issuer = "",
            Audience = "",
            DurationInMinutes = 60 
        };
        _handler = new LoginCommandHandler(
            _tokenServiceMock.Object,
            _jwtSettings,
            _authServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsException()
    {
        // Arrange
        _authServiceMock
            .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);
        var request = new LoginCommandRequest {UserName = "", Password = ""};

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsException()
    {
        // Arrange
        _authServiceMock
            .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser {FullName = ""});
        
        _authServiceMock
            .Setup(x => x.CheckPasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), false))
            .ReturnsAsync(SignInResult.Failed);
        var request = new LoginCommandRequest {UserName = "", Password = ""};

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(request, CancellationToken.None));
    }
}