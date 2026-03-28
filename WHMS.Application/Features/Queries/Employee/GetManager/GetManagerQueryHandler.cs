using System.Runtime.Serialization;
using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.Employee.GetManager;

public class GetManagerQueryHandler : IRequestHandler<GetManagerQueryRequest, GetManagerQueryResponse>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetManagerQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<GetManagerQueryResponse> Handle(GetManagerQueryRequest request, CancellationToken cancellationToken)
    {
    
        var manager = await _employeeRepository.GetManager(Guid.Parse(request.ManagerId!));
        if (manager is null)
            throw new Exception("Manager not found.");
        
        return new()
        {
            FullName = manager.FullName,
            WarehouseId = manager.WarehouseId.ToString(),
            CreatedAt = manager.CreatedAt,
            UpdatedAt = manager.UpdatedAt
        };
    }
}