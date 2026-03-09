using WHMS.Domain.Entities.Abstractions;

namespace WHMS.Domain.Entities;

public class Delivery : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid StoreId { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public Guid? ReceivedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public string CreatedByName { get; set; } = null!;
    public string CreatedByUserName { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }
    public string UpdatedByName { get; set; } = null!;
    public string UpdatedByUserName { get; set; } = null!;
    public bool IsActive { get; set; }
    
}