using WHMS.Domain.Entities.Abstractions;

namespace WHMS.Domain.Entities;

public class Shipment : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid StoreId { get; set; }
    public List<ShipmentItem>? ShipmentItems { get; set; }
    public DateTime SendingDate { get; set; }
    public Guid? SendById { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public string? CreatedByName { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }
    public string? UpdatedByName { get; set; }
    public string? UpdatedByUserName { get; set; }
    public bool IsActive { get; set; }
}