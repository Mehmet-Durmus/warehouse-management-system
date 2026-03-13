using WHMS.Domain.Entities.Abstractions;

namespace WHMS.Domain.Entities;

public class Category : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }
    public required string CategoryName { get; set; }
    public required string NormalizedCategoryName { get; set; }
    public List<SKU>? Skus { get; set; }
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