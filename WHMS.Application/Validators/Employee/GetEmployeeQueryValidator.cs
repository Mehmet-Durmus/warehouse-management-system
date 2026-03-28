using FluentValidation;
using WHMS.Application.Features.Queries.Employee.GetEmployee;

namespace WHMS.Application.Validators.Employee;

public class GetEmployeeQueryValidator : AbstractValidator<GetEmployeeQueryRequest>
{
    public GetEmployeeQueryValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("The EmployeeId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The EmployeeId must be a valid GUID.");
    }
}