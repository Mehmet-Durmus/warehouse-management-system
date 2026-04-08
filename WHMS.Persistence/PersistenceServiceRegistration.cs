using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WHMS.Persistence.SeedData.Dto;

namespace WHMS.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.Configure<LogisticDirectorDto>(
            configuration.GetSection("LogisticDirectorData"));

        services.Configure<LocationSeedDto>(options =>
        {
            var path = Path.Combine(
                environment.ContentRootPath,
                "locations.json");

            var data = JsonSerializer.Deserialize<LocationSeedDto>(
                File.ReadAllText(path),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidOperationException("Location seed data could not be deserialized from locations.json");;

            options.Cities = data.Cities;
        });

        return services;
    }
}
