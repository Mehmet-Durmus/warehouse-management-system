using Microsoft.AspNetCore.Identity;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;

namespace WHMS.Persistence.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<SignInResult> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockoutOnFailure)
        => await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure);

    public async Task<ApplicationUser?> FindByNameAsync(string userName)
        => await _userManager.FindByNameAsync(userName);

    public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        => _userManager.GetRolesAsync(user);
}