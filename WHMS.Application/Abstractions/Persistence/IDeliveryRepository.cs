using WHMS.Application.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IDeliveryRepository
{
    Task CreateDelivery(Delivery delivery);
    Task AddDeliveryItem(DeliveryItem deliveryItem);
    void UpdateDelivery(Delivery delivery);
    void UpdateDeliveryItem(DeliveryItem deliveryItem);
    Task DeleteDelivery(Guid deliveryId);
    Task DeleteDeliveryItem(Guid deliveryItemId);
    Task<List<Delivery>> GetDeliveries(DeliveryFilter filter, bool withPagination);
    Task<int> GetDeliveriesCount(DeliveryFilter filter);
    Task<Delivery> GetDelivery(Guid deliveryId);
    Task<List<DeliveryItem>> GetDeliveryItems(DeliveryItemFilter filter, bool withPagination);
    Task<int> GetDeliveryItemsCount(DeliveryItemFilter filter);
    Task<DeliveryItem> GetDeliveryItem(Guid deliveryItemId);


    Task<bool> HasIncomingDeliveriesWithSku(Guid skuId);
}