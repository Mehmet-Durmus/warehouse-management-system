using WHMS.Domain.Entities.Abstractions;
using WHMS.Domain.ValueObjects;

namespace WHMS.Domain.Entities;

public class Warehouse : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }
    public required string WarehouseName { get; set; }
    public string NormalizedName { get; set; } = null!;
    public required Address Address { get; set; }
    public List<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
    public List<Delivery> Deliveries { get; set; } = new List<Delivery>();
    public List<Shipment> Shipments { get; set; } = new List<Shipment>();
    public List<StockState> StockStates { get; set; } = new List<StockState>();
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