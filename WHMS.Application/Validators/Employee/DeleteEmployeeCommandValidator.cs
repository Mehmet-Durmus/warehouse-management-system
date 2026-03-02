using FluentValidation;
using WHMS.Application.Features.Command.Employee.DeleteEmployee;

namespace WHMS.Application.Validators.Employee;

public class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommandRequest>
{
    public DeleteEmployeeCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Provide a manager id.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("ManagerId must be a valid GUID.");
    }
}