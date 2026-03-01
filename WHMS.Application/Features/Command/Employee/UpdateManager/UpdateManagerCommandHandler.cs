using MediatR;
using Microsoft.AspNetCore.Identity;
using WHMS.Application.Abstractions.Persistence;
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
        bool isWarehouseExists = await _warehouseRepository.WarehouseExists(Guid.Parse(request.WarehouseId!));
        if (!isWarehouseExists)
            throw new Exception("Warehouse not found.");
        
        var manager = await _employeeRepository.GetManager(Guid.Parse(request.UserId!));
        if (manager is null)
            throw new Exception("Manager not found.");
        
        manager.FullName = request.FullName!;
        manager.WarehouseId = Guid.Parse(request.WarehouseId!);
        _employeeRepository.Update(manager);
        await _unitOfWork.CommitAsync();
        return new()
        {
            UserName = manager.UserName,
            FullName = manager.FullName,
            WarehouseId = manager.WarehouseId.ToString()
        };
    }
}