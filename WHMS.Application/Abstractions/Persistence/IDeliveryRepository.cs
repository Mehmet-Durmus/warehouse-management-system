using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IDeliveryRepository
{
    Task CreateDelivery(Delivery delivery);
    Task AddDeliveryItem(Guid deliveryId, DeliveryItem deliveryItem);
    void UpdateDelivery(Delivery delivery);
    void UpdateDeliveryItem(DeliveryItem deliveryItem);
    Task DeleteDelivery(Guid deliveryId);
    Task DeleteDeliveryItem(Guid deliveryItemId);
    Task<List<Delivery>> GetDeliveries();
    Task<Delivery> GetDelivery(Guid deliveryId);
    Task<List<DeliveryItem>> GetDeliveryItems();
    Task<DeliveryItem> GetDeliveryItem(Guid deliveryItem);
    Task<List<DeliveryItem>> GetDeliveryItemsByDelivery(Guid deliveryId);
}