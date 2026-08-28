using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.BusinessRules.Employee;

namespace WHMS.Application.Features.Command.Employee.DeleteEmployee;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommandRequest>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmployeeCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteEmployeeCommandRequest request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetManager(Guid.Parse(request.EmployeeId!));
        if (employee is null)
            employee = await _employeeRepository.GetStaffMember(Guid.Parse(request.EmployeeId!));

        employee = EmployeeRules.EnsureExists(employee);

        employee.IsActive = false;
        await _unitOfWork.CommitAsync();
    }
}