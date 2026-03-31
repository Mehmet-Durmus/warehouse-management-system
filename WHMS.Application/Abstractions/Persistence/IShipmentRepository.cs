
using WHMS.Application.Common.Filtering.Filters;
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
    Task<List<Shipment>> GetShipments(ShipmentFilter filter, bool applyPagination);
    Task<int> GetShipmentsCount(ShipmentFilter filter);
    Task<Shipment> GetShipment(Guid shipmentId);
    Task<List<ShipmentItem>> GetShipmentItems(ShipmentItemFilter filter, bool applyPagination);
    Task<int> GetShipmentItemsCount(ShipmentItemFilter filter);
    Task<ShipmentItem> GetShipmentItem(Guid shipmentItemId);        
}