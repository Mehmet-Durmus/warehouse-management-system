using MediatR;
using Microsoft.AspNetCore.Identity;
using WHMS.Application.Asbstractions.Infrastructure;
using WHMS.Application.Configurations;
using WHMS.Application.DTOs;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommandRequest, LoginCommandResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;
    public LoginCommandHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService, JwtSettings jwtSettings)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings;
    }

    public async Task<LoginCommandResponse> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
            throw new Exception("User not found!");
        var roles = await _userManager.GetRolesAsync(user);
        var tokenResult = _tokenService.CreateAccessToken(user, roles, _jwtSettings.DurationInMinutes);

        return new()
        {
            AccessToken = tokenResult.AccessToken,
            Expiration = tokenResult.Expiration
        };
        
    }
}