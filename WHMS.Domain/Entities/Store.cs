using WHMS.Domain.Entities.Abstractions;
using WHMS.Domain.ValueObjects;

namespace WHMS.Domain.Entities;

public class Store : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }
    public required string StoreName { get; set; }
    public required Address Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}