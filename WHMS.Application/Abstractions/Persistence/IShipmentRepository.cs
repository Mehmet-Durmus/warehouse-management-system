using WHMS.Application.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IShipmentRepository
{
    Task CreateShipment(Shipment shipment);
    Task AddShipmentItem(ShipmentItem shipmentItem);
    void UpdateShipment(Shipment shipment);
    void UpdateShipmentItem(ShipmentItem shipmentItem);
    Task DeleteShipment(Guid shipmentId);
    Task DeleteShipmentItem(Guid shipmentItemId);
    Task<List<Shipment>> GetShipments(ShipmentFilter filter, bool withPagination);
    Task<int> GetShipmentsCount(ShipmentFilter filter);
    Task<Shipment> GetShipment(Guid shipmentId);
    Task<List<ShipmentItem>> GetShipmentItems();
    Task<int> GetShipmentItemsCount();
    Task<ShipmentItem> GetShipmentItem(Guid shipmentItemId);        
}