using MediatR;

namespace WHMS.Application.Features.Queries.Employee.GetManager;

public class GetManagerQueryRequest : IRequest<GetManagerQueryResponse>
{ 
    public string? ManagerId { get; set; }
}