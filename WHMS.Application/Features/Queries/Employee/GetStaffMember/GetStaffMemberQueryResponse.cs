namespace WHMS.Application.Features.Queries.Employee.GetStaffMember;

public class GetStaffMemberQueryResponse
{
    public string? FullName { get; set; }
    public string? WarehouseId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}