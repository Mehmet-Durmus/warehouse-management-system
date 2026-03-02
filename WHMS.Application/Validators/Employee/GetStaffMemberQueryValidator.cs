using FluentValidation;
using WHMS.Application.Features.Queries.Employee.GetStaffMember;

namespace WHMS.Application.Validators.Employee;

public class GetStaffMemberQueryValidator : AbstractValidator<GetStaffMemberQueryRequest>
{
    public GetStaffMemberQueryValidator()
    {
        RuleFor(x => x.StaffMemberId)
            .NotEmpty().WithMessage("Provide a staff member id.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("StaffMemberId must be a valid GUID.");
    }
}