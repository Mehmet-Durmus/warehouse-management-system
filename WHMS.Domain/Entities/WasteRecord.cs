using WHMS.Domain.Entities.Abstractions;

namespace WHMS.Domain.Entities;

public class WasteRecord : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid SkuId { get; set; }
    public SKU? Sku { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
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