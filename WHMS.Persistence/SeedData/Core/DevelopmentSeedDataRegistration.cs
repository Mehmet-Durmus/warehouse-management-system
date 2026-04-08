using Microsoft.Extensions.DependencyInjection;
using WHMS.Persistence.SeedData.Seeders;

namespace WHMS.Persistence.SeedData.Core;

public static class DevelopmentSeedDataRegistration
{
    public static IServiceCollection AddSeedDataServices(this IServiceCollection services)
    {
        services.AddScoped<ISeeder, CatalogSeeder>();
        services.AddScoped<ISeeder, DeliverySeeder>();
        services.AddScoped<ISeeder, EmployeeSeeder>();
        services.AddScoped<ISeeder, InventoryCountSeeder>();
        services.AddScoped<ISeeder, ShipmentSeeder>();
        services.AddScoped<ISeeder, StockStateSeeder>();
        services.AddScoped<ISeeder, StoreSeeder>();
        services.AddScoped<ISeeder, WarehouseSeeder>();
        services.AddScoped<ISeeder, WasteRecordSeeder>();
        services.AddScoped<SeedDataResolver>();
        services.AddScoped<SeedRunner>();
        

        return services;
    }
}