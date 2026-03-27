using FluentValidation.Validators;
using MediatR;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Warehouse.AssignEmployees;

public class AssignEmployeesCommandHandler : IRequestHandler<AssignEmployeesCommandRequest, AssignEmployeesCommandResponse>
{
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;

    public AssignEmployeesCommandHandler(IAuthService authService, IUnitOfWork unitOfWork)
    {
        _authService = authService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AssignEmployeesCommandResponse> Handle(AssignEmployeesCommandRequest request, CancellationToken cancellationToken)
    {
        AssignEmployeesCommandResponse response = new() { Warnings = [] };
        if (request.ManagerId is not null)
        {
            var user = await _authService.FindByIdAsync(request.ManagerId);
            if (user is null)
                response.Warnings.Add($"'{request.ManagerId}' is invalid. Manager not found.");
            else
            {
                var roles = await _authService.GetRolesAsync(user);
                if (!roles.Contains("WarehouseManager"))
                    response.Warnings.Add($"{user.FullName} is not a manager.");
                else
                {
                    if (user.WarehouseId is not null)
                        response.Warnings.Add($"{user.FullName} works at a different warehouse.");
                    else
                        user.WarehouseId = Guid.Parse(request.WarehouseId);
                }
            }
        }

        if (request.StaffIds is not null && request.StaffIds.Count > 0)
            foreach (var staffId in request.StaffIds)
            {
                var user = await _authService.FindByIdAsync(staffId);
                if (user is null)
                    response.Warnings.Add($"'{staffId}' is invalid. Staff member not found.");
                else
                {
                    var roles = await _authService.GetRolesAsync(user);
                    if (!roles.Contains("WarehouseStaff"))
                        response.Warnings.Add($"{user.FullName} is not a staff member.");
                    else
                    {
                        if (user.WarehouseId is not null)
                            response.Warnings.Add($"{user.FullName} works at a different warehouse.");
                        else
                            user.WarehouseId = Guid.Parse(request.WarehouseId);
                    }
                }
                
            }

        await _unitOfWork.CommitAsync();
        return response;
    }
}