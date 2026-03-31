using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Extensions;
using WHMS.Domain.Entities;
using WHMS.Persistence.Contexts;
using WHMS.Application.Common.Filtering.Extensions;
using WHMS.Application.Common.Filtering.Filters;

namespace WHMS.Persistence.Repositories;

public class ShipmentRepository : IShipmentRepository
{
    private readonly WHMSDbContext _context;

    public ShipmentRepository(WHMSDbContext context)
    {
        _context = context;
    }

    public async Task AddShipmentItem(ShipmentItem shipmentItem)
        => await _context.ShipmentItems.AddAsync(shipmentItem);

    public async Task CreateShipment(Shipment shipment)
        => await _context.Shipments.AddAsync(shipment);

    public async Task DeleteShipment(Guid shipmentId)
    {
        var shipment = await GetShipment(shipmentId);
        shipment.IsActive = false;
    }

    public async Task DeleteShipmentItem(Guid shipmentItemId)
    {
        var shipmentItem = await GetShipmentItem(shipmentItemId);
        shipmentItem.IsActive = false;
    }

    public async Task<Shipment> GetShipment(Guid shipmentId)
    {
        var shipment = await _context.Shipments
            .Where(s => s.Id == shipmentId)
            .Include(s => s.Warehouse)
            .Include(s => s.Store)
            .Include(s => s.ShipmentItems)!
            .ThenInclude(i => i.SKU)
            .SingleOrDefaultAsync();
        
        return shipment!;
    }

    public async Task<ShipmentItem> GetShipmentItem(Guid shipmentItemId)
    {
        var item = await _context.ShipmentItems
            .Where(i => i.Id == shipmentItemId)
            .Include(i => i.SKU)
            .SingleOrDefaultAsync();

        return item!;
    }

    public async Task<List<ShipmentItem>> GetShipmentItems(ShipmentItemFilter filter, bool applyPagination)
        => await _context.ShipmentItems
            .Apply(filter, applyPagination)
            .Include(i => i.SKU)
            .ToListAsync();

    public async Task<int> GetShipmentItemsCount(ShipmentItemFilter filter)
        => await _context.ShipmentItems
            .Apply(filter, applyPagination: false)
            .CountAsync();

    public async Task<List<Shipment>> GetShipments(ShipmentFilter filter, bool applyPagination)
        => await _context.Shipments
            .Apply(filter, applyPagination)
            .Include(s => s.Warehouse)
            .Include(s => s.Store)
            .Include(s => s.ShipmentItems)!
            .ThenInclude(i => i.SKU)
            .ToListAsync();

    public async Task<int> GetShipmentsCount(ShipmentFilter filter)
        => await _context.Shipments
            .Apply(filter, false)
            .CountAsync();

    public void UpdateShipment(Shipment shipment)
        => _context.Shipments.Update(shipment);

    public void UpdateShipmentItem(ShipmentItem shipmentItem)
        => _context.ShipmentItems.Update(shipmentItem);
}