using Microsoft.AspNetCore.Identity;
using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IAuthService
{
    Task<ApplicationUser?> FindByNameAsync(string userName);
    Task<SignInResult> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockoutOnFailure);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task <ApplicationUser?> FindByIdAsync(string userId);
    Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
}