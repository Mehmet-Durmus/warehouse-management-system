using WHMS.Domain.Entities.Abstractions;

namespace WHMS.Domain.Entities;

public class SKU : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }
    public required string SKUName { get; set; }
    public required string NormalizedSKUName { get; set; }
    public required string Barcode { get; set; }
    public decimal UnitPrice { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}