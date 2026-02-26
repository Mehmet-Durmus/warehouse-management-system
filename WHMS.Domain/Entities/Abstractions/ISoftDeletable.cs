namespace WHMS.Domain.Entities.Abstractions;

public interface ISoftDeletable
{
    public bool IsActive { get; set; }
}