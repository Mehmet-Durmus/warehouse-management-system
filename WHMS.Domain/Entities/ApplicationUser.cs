using Microsoft.AspNetCore.Identity;
using WHMS.Domain.Entities.Abstractions;

namespace WHMS.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, IAuditable, ISoftDeletable
{
    public string FullName { get; set; } = null!;
    public Guid? WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public string? CreatedByName { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }
    public string? UpdatedByName { get; set; }
    public string? UpdatedByUserName { get; set; }
}