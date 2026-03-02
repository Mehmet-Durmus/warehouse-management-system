using FluentValidation;
using WHMS.Application.Features.Queries.Employee.GetManager;

namespace WHMS.Application.Validators.Employee;

public class GetManagerQueryValidator : AbstractValidator<GetManagerQueryRequest>
{
    public GetManagerQueryValidator()
    {
        RuleFor(x => x.ManagerId)
            .NotEmpty().WithMessage("Provide a manager id.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("ManagerId must be a valid GUID.");
    }
}