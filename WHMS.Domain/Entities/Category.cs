using WHMS.Domain.Entities.Abstractions;

namespace WHMS.Domain.Entities;

public class Category : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }
    public required string CategoryName { get; set; }
    public required string NormalizedCategoryName { get; set; }
    public List<SKU>? Skus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}