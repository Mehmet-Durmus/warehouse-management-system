using WHMS.Domain.Entities.Abstractions;

namespace WHMS.Domain.Entities;

public class Delivery : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    public List<DeliveryItem>? DeliveryItems { get; set; }
    public DateTime ExpectedArrivalDate { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public Guid? ReceivedById { get; set; }
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