using FluentValidation;
using WHMS.Application.Features.Command.Employee.CreateEmployee;

namespace WHMS.Application.Validators.Employee;

public class CreateManagerCommandValidator : AbstractValidator<CreateManagerCommandRequest>
{
    public CreateManagerCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .Matches(@"^\p{L}{2,}(?:\s\p{L}{2,})+$")
            .WithMessage("Full name must contain at least two words, each with minimum two letters.");

        // RuleFor(x => x.WarehouseId)
        //     .NotEmpty().WithMessage("WarehouseId is required.")
        //     .Must(x => Guid.TryParse(x, out _)).WithMessage("WarehouseId must be a valid GUID.");
    }
}