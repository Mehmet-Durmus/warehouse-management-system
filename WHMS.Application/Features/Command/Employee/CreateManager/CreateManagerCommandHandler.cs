using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Features.Command.Employee.CreateEmployee;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Employee.CreateManager;

public class CreateManagerCommandHandler : IRequestHandler<CreateManagerCommandRequest, CreateManagerCommandResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAuthService _authService;
    private readonly IPasswordCreator _passwordCreator;
    private readonly IWarehouseRepository _warehouseRepository;

    public CreateManagerCommandHandler(IEmployeeRepository employeeRepository, IAuthService authService, IPasswordCreator passwordCreator, IWarehouseRepository warehouseRepository)
    {
        _employeeRepository = employeeRepository;
        _authService = authService;
        _passwordCreator = passwordCreator;
        _warehouseRepository = warehouseRepository;
    }

    public async Task<CreateManagerCommandResponse> Handle(CreateManagerCommandRequest request, CancellationToken cancellationToken)
    {
        string userName = await _employeeRepository.GenerateWarehouseManagerUserName();
        var manager = new ApplicationUser 
        {
            FullName = request.FullName!,
            UserName = userName,
            Email = userName,
            EmailConfirmed = true
        };

        if (!string.IsNullOrWhiteSpace(request.WarehouseId))
        {
            bool isWarehouseExists = await _warehouseRepository.WarehouseExists(Guid.Parse(request.WarehouseId!));
            if (!isWarehouseExists)
                throw new Exception("Warehouse not found.");
                
            bool hasWarehouseAnyManager = await _employeeRepository.HasWarehouseAnyManager(Guid.Parse(request.WarehouseId!));
            if (hasWarehouseAnyManager)
                throw new Exception("The warehouse has already a manager.");
            
            manager.WarehouseId = Guid.Parse(request.WarehouseId);
        }

        var tempPassword = await _passwordCreator.CreateTempPassword();
        var result = await _authService.CreateAsync(manager, tempPassword);
        if (result.Succeeded)
            await _authService.AddToRoleAsync(manager, ApplicationRole.WarehouseManager);
        else
            throw new Exception("Manager could not be created.");
        return new()
        {
            UserId = manager.Id.ToString(),
            UserName = manager.UserName,
            FullName = manager.FullName,
            WarehouseId = manager.WarehouseId?.ToString(),
            TempPassword = tempPassword
        };
    }
}