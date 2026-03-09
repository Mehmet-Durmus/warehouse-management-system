namespace WHMS.Domain.Entities.Abstractions;

public interface IAuditable
{
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public string CreatedByName { get; set; }
    public string CreatedByUserName { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }
    public string UpdatedByName { get; set; }
    public string UpdatedByUserName { get; set; }
}