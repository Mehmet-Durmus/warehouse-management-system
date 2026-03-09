using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;

namespace WHMS.Persistence.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    public Task AddDeliveryItem(Guid deliveryId, DeliveryItem deliveryItem)
    {
        throw new NotImplementedException();
    }

    public Task CreateDelivery(Delivery delivery)
    {
        throw new NotImplementedException();
    }

    public Task DeleteDelivery(Guid deliveryId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteDeliveryItem(Guid deliveryItemId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Delivery>> GetDeliveries()
    {
        throw new NotImplementedException();
    }

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