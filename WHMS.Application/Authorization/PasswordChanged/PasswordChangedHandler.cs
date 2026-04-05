using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Authorization.PasswordChanged;

public class PasswordChangedHandler : AuthorizationHandler<PasswordChangedRequirement>
{
    private readonly IAuthService _authService;

    public PasswordChangedHandler(IAuthService authService)
    {
        _authService = authService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PasswordChangedRequirement requirement)
    {
        var isPasswordChanged = context.User.FindFirst("IsPasswordChanged")?.Value;

        if (isPasswordChanged != "True")
            return;

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var tokenPasswordChangedAt = context.User.FindFirst("PasswordChangedAt")?.Value;

        if (userId == null || tokenPasswordChangedAt == null)
            return;

        var user = await _authService.FindByIdAsync(userId);

        if (user == null)
            return;

        if (user.PasswordChangedAt.ToString("O") == tokenPasswordChangedAt)
            context.Succeed(requirement);
    }
}