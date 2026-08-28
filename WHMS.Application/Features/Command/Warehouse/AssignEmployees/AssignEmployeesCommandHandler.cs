using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Domain.BusinessRules.Warehouse;

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
            bool hasManagerRole = user is not null && (await _authService.GetRolesAsync(user)).Contains(ApplicationRole.WarehouseManager);

            var warning = WarehouseRules.ValidateManagerAssignment(user, hasManagerRole, request.ManagerId);
            if (warning is not null)
                response.Warnings.Add(warning);
            else
                user!.WarehouseId = Guid.Parse(request.WarehouseId);
        }

        if (request.StaffIds is not null && request.StaffIds.Count > 0)
            foreach (var staffId in request.StaffIds)
            {
                var user = await _authService.FindByIdAsync(staffId);
                bool hasStaffRole = user is not null && (await _authService.GetRolesAsync(user)).Contains(ApplicationRole.WarehouseStaff);

                var warning = WarehouseRules.ValidateStaffAssignment(user, hasStaffRole, staffId);
                if (warning is not null)
                    response.Warnings.Add(warning);
                else
                    user!.WarehouseId = Guid.Parse(request.WarehouseId);
            }

        await _unitOfWork.CommitAsync();
        return response;
    }
}