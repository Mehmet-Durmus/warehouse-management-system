namespace WHMS.Application.Abstractions.Infrastructure;

public interface ICurrentUserService
{
    public Guid? UserId { get; }
    public string? FullName { get; }
    public string? UserName { get; }
    public string? WarehouseId { get; }
}