using FluentValidation;
using WHMS.Application.Features.Command.Employee.UpdateEmployeePassword;

namespace WHMS.Application.Validators.Employee;

public class UpdateEmployeePasswordCommandValidator : AbstractValidator<UpdateEmployeePasswordCommandRequest>
{
    public UpdateEmployeePasswordCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("The EmployeeId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The EmployeeId must be a valid GUID.");
    }
}