namespace WHMS.Application.Features.Queries.CurrentUserInfo;

public class CurrentUserInfoQueryResponse
{
    public string? WarehouseId { get; set; }
    public string UserName { get; set; } = null!;
    public string FullName { get; set; } = null!;
}