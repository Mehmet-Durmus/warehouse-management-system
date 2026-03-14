using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;

namespace WHMS.Persistence.Repositories;

public class ShipmentRepository : IShipmentRepository
{
    public Task AddShipmentItem(ShipmentItem shipmentItem)
    {
        throw new NotImplementedException();
    }

    public Task CreateShipment(Shipment shipment)
    {
        throw new NotImplementedException();
    }

    public Task DeleteShipment(Guid shipmentId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteShipmentItem(Guid shipmentItemId)
    {
        throw new NotImplementedException();
    }

    public Task<Shipment> GetShipment(Guid shipmentId)
    {
        throw new NotImplementedException();
    }

    public Task<ShipmentItem> GetShipmentItem(Guid shipmentItemId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ShipmentItem>> GetShipmentItems()
    {
        throw new NotImplementedException();
    }

    public Task<int> GetShipmentItemsCount()
    {
        throw new NotImplementedException();
    }

    public Task<List<Shipment>> GetShipments()
    {
        throw new NotImplementedException();
    }

    public Task<int> GetShipmentsCount()
    {
        throw new NotImplementedException();
    }

    public void UpdateShipment(Shipment shipment)
    {
        throw new NotImplementedException();
    }

    public void UpdateShipmentItem(ShipmentItem shipmentItem)
    {
        throw new NotImplementedException();
    }
}