using MediatR;
using Microsoft.AspNetCore.Identity;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Configurations;
using WHMS.Application.DTOs;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommandRequest, LoginCommandResponse>
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;
    public LoginCommandHandler(ITokenService tokenService, JwtSettings jwtSettings, IAuthService authService)
    {
        _tokenService = tokenService;
        _jwtSettings = jwtSettings;
        _authService = authService;
    }

    public async Task<LoginCommandResponse> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await _authService.FindByNameAsync(request.UserName);
        if (user == null)
            throw new Exception("Invalid credantials!");
        
        SignInResult result = await _authService.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            throw new Exception("Invalid credantials!");

        var roles = await _authService.GetRolesAsync(user);
        var tokenResult = _tokenService.CreateAccessToken(user, roles, _jwtSettings.DurationInMinutes);

        return new()
        {
            AccessToken = tokenResult.AccessToken,
            Expiration = tokenResult.Expiration
        };
    }
}