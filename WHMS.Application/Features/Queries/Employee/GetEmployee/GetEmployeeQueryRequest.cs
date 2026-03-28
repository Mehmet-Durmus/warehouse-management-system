using MediatR;

namespace WHMS.Application.Features.Queries.Employee.GetEmployee;

public class GetEmployeeQueryRequest : IRequest<GetEmployeeQueryResponse>
{
    public string? EmployeeId { get; set; }
}