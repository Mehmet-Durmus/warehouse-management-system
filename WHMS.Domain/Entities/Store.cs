using WHMS.Domain.Entities.Abstractions;
using WHMS.Domain.ValueObjects;

namespace WHMS.Domain.Entities;

public class Store : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }
    public string StoreName { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
    public Address Address { get; set; } = null!;
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