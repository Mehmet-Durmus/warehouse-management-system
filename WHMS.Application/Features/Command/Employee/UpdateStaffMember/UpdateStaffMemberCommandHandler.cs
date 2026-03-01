using MediatR;
using WHMS.Application.Abstractions.Persistence;

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
        bool isWarehouseExists = await _warehouseRepository.WarehouseExists(Guid.Parse(request.WarehouseId!));
        if (!isWarehouseExists)
            throw new Exception("Warehouse not found.");

        var staffMember = await _employeeRepository.GetStaffMember(Guid.Parse(request.UserId!));
        if (staffMember is null)
            throw new Exception("Staff member not found.");

        staffMember.FullName = request.FullName!;
        staffMember.WarehouseId = Guid.Parse(request.WarehouseId!);
        _employeeRepository.Update(staffMember);
        await _unitOfWork.CommitAsync();
        return new()
        {
            UserName = staffMember.UserName,
            FullName = staffMember.FullName,
            WarehouseId = staffMember.WarehouseId.ToString()
        };
    }
}