using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WHMS.Application.Common.Constants;
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
        foreach (var role in ApplicationRole.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid> {Name = role});
        }

        // Logistic Director
        var directorSettings = seedOptions.Value.LogisticDirectorData;
        const string directorRoleName = "LogisticDirector";

        var existingDirector = await userManager.FindByNameAsync(directorSettings!.UserName!);
        if (existingDirector == null)
        {
            var user = new ApplicationUser 
            {
                FullName = "",
                UserName = directorSettings.UserName,
                Email = directorSettings.UserName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, directorSettings.Password!);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, directorRoleName);
            else
                throw new Exception("Director could not be created: " +
                    string.Join(",", result.Errors.Select(e => e.Description)));
        }

        // Locations Data
        if (!context.Cities.Any())
        {
            var json = await File.ReadAllTextAsync("locations.json");
            var root = JsonNode.Parse(json);
            if (root != null)
            {
                foreach (var city in root["Cities"]!.AsArray())
                {
                    var cityEntity = new City {Name = city!["Name"]!.ToString()};
                    await context.Cities.AddAsync(cityEntity);

                    foreach (var district in city["Districts"]!.AsArray())
                    {
                        var districtEntity = new District 
                        {
                            Name = district!["Name"]!.ToString(),
                            City = cityEntity
                        };
                        await context.Districts.AddAsync(districtEntity);

                        foreach (var neighborhood in district["Neighborhoods"]!.AsArray())
                        {
                            var neighborhoodEntity = new Neighborhood
                            {
                                Name = neighborhood!["Name"]!.ToString(),
                                District = districtEntity
                            };
                            await context.Neighborhoods.AddAsync(neighborhoodEntity);
                        }
                    }
                }
                await context.SaveChangesAsync();
            }
        }
        
    }
}