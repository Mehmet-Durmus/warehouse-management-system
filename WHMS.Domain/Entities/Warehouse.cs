using WHMS.Domain.Entities.Abstractions;
using WHMS.Domain.ValueObjects;

namespace WHMS.Domain.Entities;

public class Warehouse : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }
    public required string WarehouseName { get; set; }
    public required Address Address { get; set; }
    public List<ApplicationUser>? ApplicationUsers { get; set; }
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