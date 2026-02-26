using Microsoft.AspNetCore.Identity;
using WHMS.Domain.Entities.Abstractions;

namespace WHMS.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, IAuditable, ISoftDeletable
{
    public required string FullName { get; set; }
    public Guid? WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}