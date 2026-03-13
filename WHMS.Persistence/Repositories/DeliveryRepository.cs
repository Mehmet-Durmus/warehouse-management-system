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

    public async Task DeleteDelivery(Guid deliveryId)
    {
        var delivery = await GetDelivery(deliveryId);
        delivery.IsActive = false;
        UpdateDelivery(delivery);
    }

    public async Task DeleteDeliveryItem(Guid deliveryItemId)
    {
       var deliveryItem = await GetDeliveryItem(deliveryItemId);
       deliveryItem.IsActive = false;
       UpdateDeliveryItem(deliveryItem);
    }

    public async Task<List<Delivery>> GetDeliveries(DeliveryFilter filter, bool withPagination)
        => await _context.Deliveries
            .ApplyDeliveryFilter(filter, withPagination)
            .Include(d => d.DeliveryItems)!
            .ThenInclude(i => i.Sku)
            .ToListAsync();

    public async Task<int> GetDeliveriesCount(DeliveryFilter filter)
        => await _context.Deliveries
            .ApplyDeliveryFilter(filter, false)
            .Include(d => d.DeliveryItems)!
            .ThenInclude(i => i.Sku)
            .CountAsync();

    public async Task<Delivery> GetDelivery(Guid deliveryId)
    {
        var delivery = await _context.Deliveries
            .Where(d => d.Id == deliveryId)
            .Include(d => d.DeliveryItems)!
            .ThenInclude(i => i.Sku)
            .SingleOrDefaultAsync();
        return delivery!;
    }

    public async Task<DeliveryItem> GetDeliveryItem(Guid deliveryItemId)
    {
        var deliveryItem = await _context.DeliveryItems
            .Where(i => i.Id == deliveryItemId)
            .Include(i => i.Sku)
            .SingleOrDefaultAsync();
        return deliveryItem!;
    }

    public async Task<List<DeliveryItem>> GetDeliveryItems(DeliveryItemFilter filter, bool withPagination)
        => await _context.DeliveryItems
            .ApplyDeliveryItemFilter(filter, withPagination)
            .Include(i => i.Sku)
            .ToListAsync();

    public async Task<int> GetDeliveryItemsCount(DeliveryItemFilter filter)
        => await _context.DeliveryItems
            .ApplyDeliveryItemFilter(filter, false)
            .Include(i => i.Sku)
            .CountAsync();

    public void UpdateDelivery(Delivery delivery)
        => _context.Deliveries.Update(delivery);

    public void UpdateDeliveryItem(DeliveryItem deliveryItem)
        => _context.DeliveryItems.Update(deliveryItem);
}