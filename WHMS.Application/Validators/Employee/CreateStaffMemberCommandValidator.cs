using FluentValidation;
using WHMS.Application.Features.Command.Employee.CreateStaffMember;

namespace WHMS.Application.Validators.Employee;

public class CreateStaffMemberCommandValidator : AbstractValidator<CreateStaffMemberCommandRequest>
{
    public CreateStaffMemberCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .Matches(@"^\p{L}{2,}(?:\s\p{L}{2,})+$")
            .WithMessage("Full name must contain at least two words, each with minimum two letters.");

        RuleFor(x => x.WarehouseId)
            .Must(x => string.IsNullOrWhiteSpace(x) || Guid.TryParse(x, out _))
            .WithMessage("WarehouseId must be a valid GUID.");
    }
}