using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.Employee;
using WHMS.Domain.BusinessRules.Warehouse;

namespace WHMS.Application.Features.Command.Employee.UpdateStaffMember;

public class UpdateStaffMemberCommandHandler : IRequestHandler<UpdateStaffMemberCommandRequest, UpdateStaffMemberCommandResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStaffMemberCommandHandler(IEmployeeRepository employeeRepository, IWarehouseRepository warehouseRepository, IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateStaffMemberCommandResponse> Handle(UpdateStaffMemberCommandRequest request, CancellationToken cancellationToken)
    {
        var staffMember = EmployeeRules.EnsureStaffMemberExists(await _employeeRepository.GetStaffMember(Guid.Parse(request.UserId!)));

        if (!string.IsNullOrWhiteSpace(request.WarehouseId))
        {
            WarehouseRules.EnsureExists(await _warehouseRepository.WarehouseExists(Guid.Parse(request.WarehouseId!)));
            staffMember.WarehouseId = Guid.Parse(request.WarehouseId);
        }
        staffMember.FullName = request.FullName!;

        await _unitOfWork.CommitAsync();
        return new()
        {
            UserName = staffMember.UserName,
            FullName = staffMember.FullName,
            WarehouseId = staffMember.WarehouseId.ToString()
        };
    }
}