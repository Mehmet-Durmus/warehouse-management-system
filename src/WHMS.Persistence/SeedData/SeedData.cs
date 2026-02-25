using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WHMS.Domain.Entities;
using WHMS.Persistence.Contexts;
using WHMS.Persistence.SeedData;

namespace WHMS.Persistence.SeedData;

public static class SeedData
{
    public static async Task InitializeAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<WHMSDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var seedOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedSettings>>();

        // Roles
        List<string> roles =  ["Admin", "LogisticDirector", "WarehouseManager", "WarehouseStaff"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid> {Name = role});
        }

        // Logistic Director
        var directorSettings = seedOptions.Value.LogisticDirectorData;
        const string directorRoleName = "LogisticDirector";

        var existingDirector = await userManager.FindByNameAsync(directorSettings.UserName);
        if (existingDirector == null)
        {
            var user = new ApplicationUser 
            {
                FullName = "",
                UserName = directorSettings.UserName,
                Email = directorSettings.UserName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, directorSettings.Password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, directorRoleName);
            else
                throw new Exception("Director oluşturulamadı: " +
                    string.Join(",", result.Errors.Select(e => e.Description)));
        }
    }
}