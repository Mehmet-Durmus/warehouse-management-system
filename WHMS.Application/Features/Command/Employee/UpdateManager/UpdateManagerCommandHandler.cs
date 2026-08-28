using MediatR;
using Microsoft.AspNetCore.Identity;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.Employee;
using WHMS.Domain.BusinessRules.Warehouse;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Employee.UpdateManager;

public class UpdateManagerCommandHandler : IRequestHandler<UpdateManagerCommandRequest, UpdateManagerCommandResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateManagerCommandHandler(IEmployeeRepository employeeRepository, IWarehouseRepository warehouseRepository, IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateManagerCommandResponse> Handle(UpdateManagerCommandRequest request, CancellationToken cancellationToken)
    {
        var manager = EmployeeRules.EnsureManagerExists(await _employeeRepository.GetManager(Guid.Parse(request.UserId!)));

        if (!string.IsNullOrWhiteSpace(request.WarehouseId))
        {
            WarehouseRules.EnsureExists(await _warehouseRepository.WarehouseExists(Guid.Parse(request.WarehouseId!)));
            manager.WarehouseId = Guid.Parse(request.WarehouseId!);
        }
        manager.FullName = request.FullName!;

        await _unitOfWork.CommitAsync();
        return new()
        {
            UserName = manager.UserName,
            FullName = manager.FullName,
            WarehouseId = manager.WarehouseId.ToString()
        };
    }
}