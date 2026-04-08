using System.Formats.Asn1;
using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Persistence.Contexts;
using WHMS.Persistence.SeedData.Core;
using WHMS.Persistence.SeedData.Dto;

namespace WHMS.Persistence.SeedData.Seeders;

public class DeliverySeeder : ISeeder
{
    private readonly WHMSDbContext _context;
    private readonly IJsonDataLoader _jsonDataLoader;
    private readonly SeedDataResolver _seedDataResolver;

    public DeliverySeeder(WHMSDbContext context, IJsonDataLoader jsonDataLoader, SeedDataResolver seedDataResolver)
    {
        _context = context;
        _jsonDataLoader = jsonDataLoader;
        _seedDataResolver = seedDataResolver;
    }
    public int Order => 5;

    public async Task SeedAsync()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "SeedData", "Data", "deliveries.json");
        var deliveries = _jsonDataLoader.Load<DeliveriesSeedDto>(path);
        var director = await _seedDataResolver.Employee(deliveries.CreatedBy);

        if (!await _context.Deliveries.AnyAsync())
            foreach (var delivery in deliveries.Deliveries)
            {
                Domain.Entities.Delivery newDelivery = new()
                {
                    WarehouseId = await _seedDataResolver.WarehouseId(delivery.Warehouse),
                    ExpectedArrivalDate = delivery.ExpectedArrivalDate,
                    ReceivedAt = delivery.ReceivedAt,
                    ReceivedById = await _seedDataResolver.EmployeeId(delivery.ReceivedBy),
                    CreatedAt = delivery.CreatedAt,
                    CreatedById = director!.Id,
                    CreatedByName = director.FullName,
                    CreatedByUserName = director.UserName,
                    UpdatedAt = delivery.CreatedAt,
                    UpdatedById = director!.Id,
                    UpdatedByName = director.FullName,
                    UpdatedByUserName = director.UserName
                };
                _context.Deliveries.Add(newDelivery);

                foreach (var item in delivery.DeliveryItems)
                    _context.DeliveryItems.Add(new Domain.Entities.DeliveryItem
                    {
                        DeliveryId = newDelivery.Id,
                        SkuId = await _seedDataResolver.SkuId(item.SkuBarcode),
                        Quantity = item.Quantity,
                        CreatedAt = delivery.CreatedAt,
                        CreatedById = director!.Id,
                        CreatedByName = director.FullName,
                        CreatedByUserName = director.UserName,
                        UpdatedAt = delivery.CreatedAt,
                        UpdatedById = director!.Id,
                        UpdatedByName = director.FullName,
                        UpdatedByUserName = director.UserName
                    });
            }

    }
}