using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Persistence.Contexts;
using WHMS.Persistence.SeedData.Core;
using WHMS.Persistence.SeedData.Dto;

namespace WHMS.Persistence.SeedData.Seeders;

public class ShipmentSeeder : ISeeder
{
    private readonly WHMSDbContext _context;
    private readonly IJsonDataLoader _jsonDataLoader;
    private readonly SeedDataResolver _seedDataResolver;

    public ShipmentSeeder(WHMSDbContext context, IJsonDataLoader jsonDataLoader, SeedDataResolver seedDataResolver)
    {
        _context = context;
        _jsonDataLoader = jsonDataLoader;
        _seedDataResolver = seedDataResolver;
    }
    public int Order => 6;

    public async Task SeedAsync()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "SeedData", "Data", "shipments.json");
        var shipmentData = _jsonDataLoader.Load<ShipmentsSeedDto>(path);
        var director = await _seedDataResolver.Employee(shipmentData.CreatedBy);

        if (!await _context.Shipments.AnyAsync())
        {
            foreach (var shipment in shipmentData.Shipments)
            {
                var sentBy = await _seedDataResolver.Employee(shipment.SentBy);
                Domain.Entities.Shipment newShipment = new()
                {
                    WarehouseId = await _seedDataResolver.WarehouseId(shipment.Warehouse),
                    StoreId = await _seedDataResolver.StoreId(shipment.Store),
                    ExpectedSendingDate = shipment.ExpectedSendingDate,
                    SendingDate = shipment.SentAt,
                    SentById = sentBy!.Id,
                    CreatedAt = shipment.CreatedAt,
                    CreatedById = director!.Id,
                    CreatedByName = director.FullName,
                    CreatedByUserName = director.UserName,
                    UpdatedAt = shipment.CreatedAt,
                    UpdatedById = director!.Id,
                    UpdatedByName = director.FullName,
                    UpdatedByUserName = director.UserName,
                };
                
                _context.Shipments.Add(newShipment);

                foreach (var item in shipment.ShipmentItems)
                {
                    Domain.Entities.ShipmentItem newItem = new()
                    {
                        ShipmentId = newShipment.Id,
                        SkuId = await _seedDataResolver.SkuId(item.SkuBarcode),
                        Quantity = item.Quantity,
                        CreatedAt = shipment.CreatedAt,
                        CreatedById = director!.Id,
                        CreatedByName = director.FullName,
                        CreatedByUserName = director.UserName,
                        UpdatedAt = shipment.CreatedAt,
                        UpdatedById = director!.Id,
                        UpdatedByName = director.FullName,
                        UpdatedByUserName = director.UserName
                    };
                    _context.ShipmentItems.Add(newItem);
                }
            }
        }
        
        await _context.SaveChangesAsync();
    }
}