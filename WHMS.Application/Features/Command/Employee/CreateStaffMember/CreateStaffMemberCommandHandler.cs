using MediatR;
using Microsoft.AspNetCore.Identity;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Employee.CreateStaffMember;

public class CreateStaffMemberCommandHandler : IRequestHandler<CreateStaffMemberCommandRequest, CreateStaffMemberCommandResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPasswordCreator _passwordCreator;
    private readonly IWarehouseRepository _warehouseRepository;

    public CreateStaffMemberCommandHandler(IEmployeeRepository employeeRepository, UserManager<ApplicationUser> userManager, IPasswordCreator passwordCreator, IWarehouseRepository warehouseRepository)
    {
        _employeeRepository = employeeRepository;
        _userManager = userManager;
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
            bool isWarehouseExists = await _warehouseRepository.WarehouseExists(Guid.Parse(request.WarehouseId!));
            if (!isWarehouseExists)
                throw new Exception("Watehouse not found.");
            staff.WarehouseId = Guid.Parse(request.WarehouseId);
        }

        string tempPassword = await _passwordCreator.CreateTempPassword();
        var result = await _userManager.CreateAsync(staff, tempPassword);
        if (result.Succeeded)
            await _userManager.AddToRoleAsync(staff, ApplicationRole.WarehouseStaff);
        else
            throw new Exception("Staff member could not be created.");
        
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