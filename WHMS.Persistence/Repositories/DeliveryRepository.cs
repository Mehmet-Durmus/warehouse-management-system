using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Extensions;
using WHMS.Application.Filters;
using WHMS.Domain.Entities;
using WHMS.Persistence.Contexts;

namespace WHMS.Persistence.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly WHMSDbContext _context;

    public DeliveryRepository(WHMSDbContext context)
    {
        _context = context;
    }

    public async Task AddDeliveryItem(DeliveryItem deliveryItem)
        => await _context.DeliveryItems.AddAsync(deliveryItem);

    public async Task CreateDelivery(Delivery delivery)
        => await _context.Deliveries.AddAsync(delivery);

    public Task DeleteDelivery(Guid deliveryId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteDeliveryItem(Guid deliveryItemId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Delivery>> GetDeliveries(DeliveryFilter filter)
        => await _context.Deliveries
            .ApplyDeliveryFilter(filter)
            .Include(d => d.DeliveryItems)!
            .ThenInclude(i => i.Sku)
            .ToListAsync();

    public async Task<int> GetDeliveriesCount(DeliveryFilter filter)
        => await _context.Deliveries
            .ApplyDeliveryFilter(filter, false)
            .Include(d => d.DeliveryItems)!
            .ThenInclude(i => i.Sku)
            .CountAsync();

    public Task<Delivery> GetDelivery(Guid deliveryId)
    {
        throw new NotImplementedException();
    }

    public Task<DeliveryItem> GetDeliveryItem(Guid deliveryItem)
    {
        throw new NotImplementedException();
    }

    public Task<List<DeliveryItem>> GetDeliveryItems()
    {
        throw new NotImplementedException();
    }

    public Task<List<DeliveryItem>> GetDeliveryItemsByDelivery(Guid deliveryId)
    {
        throw new NotImplementedException();
    }

    public void UpdateDelivery(Delivery delivery)
    {
        throw new NotImplementedException();
    }

    public void UpdateDeliveryItem(DeliveryItem deliveryItem)
    {
        throw new NotImplementedException();
    }
}