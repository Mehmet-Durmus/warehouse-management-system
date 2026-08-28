using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Domain.BusinessRules.Employee;
using WHMS.Domain.BusinessRules.Warehouse;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Employee.CreateStaffMember;

public class CreateStaffMemberCommandHandler : IRequestHandler<CreateStaffMemberCommandRequest, CreateStaffMemberCommandResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAuthService _authService;
    private readonly IPasswordCreator _passwordCreator;
    private readonly IWarehouseRepository _warehouseRepository;

    public CreateStaffMemberCommandHandler(IEmployeeRepository employeeRepository, IAuthService authService, IPasswordCreator passwordCreator, IWarehouseRepository warehouseRepository)
    {
        _employeeRepository = employeeRepository;
        _authService = authService;
        _passwordCreator = passwordCreator;
        _warehouseRepository = warehouseRepository;
    }

    public async Task<CreateStaffMemberCommandResponse> Handle(CreateStaffMemberCommandRequest request, CancellationToken cancellationToken)
    {
        

        string userName = await _employeeRepository.GenerateWarehouseStaffUserName();
        var staff = new ApplicationUser
        {
            FullName = request.FullName!,
            UserName = userName,
            Email = userName,
            EmailConfirmed = true
        };

        if (!string.IsNullOrWhiteSpace(request.WarehouseId))
        {
            WarehouseRules.EnsureExists(await _warehouseRepository.WarehouseExists(Guid.Parse(request.WarehouseId!)));
            staff.WarehouseId = Guid.Parse(request.WarehouseId);
        }

        string tempPassword = await _passwordCreator.CreateTempPassword();
        var result = await _authService.CreateAsync(staff, tempPassword);
        EmployeeRules.EnsureStaffMemberWasCreated(result.Succeeded);
        await _authService.AddToRoleAsync(staff, ApplicationRole.WarehouseStaff);
        
        return new()
        {
            UserId = staff.Id.ToString(),
            UserName = staff.UserName,
            FullName = staff.FullName,
            WarehouseId = staff.WarehouseId?.ToString(),
            TempPassword = tempPassword
        };
    }
}