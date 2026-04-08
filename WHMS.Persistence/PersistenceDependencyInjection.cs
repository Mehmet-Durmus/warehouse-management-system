using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Persistence.Contexts;
using WHMS.Persistence.Repositories;
using WHMS.Persistence.SeedData.Dto;
using WHMS.Persistence.Services;

namespace WHMS.Persistence;

public static class PersistenceDependencyInjection
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

        services.AddDbContext<WHMSDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();
        services.AddScoped<IShipmentRepository, ShipmentRepository>();
        services.AddScoped<IInventoryCountRepository, InventoryCountRepository>();
        services.AddScoped<IWasteRecordRepository, WasteRecordRepository>();
        services.AddScoped<IStockStateRepository, StockStateRepository>();

        return services;
    }
}
