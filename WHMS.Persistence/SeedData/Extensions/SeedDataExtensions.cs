using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WHMS.Application.Common.Constants;
using WHMS.Domain.Entities;
using WHMS.Persistence.Contexts;
using WHMS.Persistence.SeedData.Core;
using WHMS.Persistence.SeedData.Dto;

namespace WHMS.Persistence.SeedData.Extensions;

public static class SeedDataExtensions
{

    public static async Task SeedDevelopmentDataAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<WHMSDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var logisticDirector = scope.ServiceProvider.GetRequiredService<IOptions<LogisticDirectorDto>>().Value;
        var locationData = scope.ServiceProvider.GetRequiredService<IOptions<LocationSeedDto>>().Value;

        // Roles
        foreach (var role in ApplicationRole.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid> {Name = role});
        }

        // Logistic Director
        if (await userManager.FindByNameAsync(logisticDirector.UserName!) == null)
        {
            ApplicationUser user = new()
            {
                FullName = logisticDirector.FullName,
                UserName = logisticDirector.UserName,
                IsPasswordChanged = true
            };

            var result = await userManager.CreateAsync(user, logisticDirector.Password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, ApplicationRole.LogisticDirector);
            else
                throw new Exception("Director could not be created:" + 
                string.Join(",", result.Errors.Select(e => e.Description)));
        }

        // Locations
        if (!await context.Cities.AnyAsync())
        {
            foreach (var city in locationData.Cities)
            {
                Domain.Entities.City newCity = new() { Name = city.Name };
                context.Cities.Add(newCity);

                foreach (var district in city.Districts)
                {
                    Domain.Entities.District newDistrict = new()
                    {
                        Name = district.Name,
                        City = newCity
                    };
                    context.Districts.Add(newDistrict);

                    foreach (var neighborhood in district.Neighborhoods)
                        context.Neighborhoods.Add( new()
                        {
                            Name = neighborhood.Name,
                            District = newDistrict
                        });
                }
            }
        }
        
        await context.SaveChangesAsync();

    }

    public static async Task SeedEssentialDataAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateAsyncScope();
        var runner = scope.ServiceProvider.GetRequiredService<SeedRunner>();
        await runner.RunAsync();
    }
}