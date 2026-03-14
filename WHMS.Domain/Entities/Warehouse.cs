using WHMS.Domain.Entities.Abstractions;
using WHMS.Domain.ValueObjects;

namespace WHMS.Domain.Entities;

public class Warehouse : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }
    public required string WarehouseName { get; set; }
    public required Address Address { get; set; }
    public List<ApplicationUser>? ApplicationUsers { get; set; }
    public List<Delivery>? Deliveries { get; set; }
    public List<Shipment>? Shipments { get; set; }
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