using FluentValidation;
using WHMS.Application.Features.Queries.InventoryCount.GetInventoryCount;

namespace WHMS.Application.Validators.InventoryCount;

public  class GetInventoryCountQueryValidator : AbstractValidator<GetInventoryCountQueryRequest>
{
    public GetInventoryCountQueryValidator()
    {
        RuleFor(x => x.InventoryCountId)
            .NotEmpty().WithMessage("The InventoryCountId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The InventoryCountId is must be a valid GUID.");
    }
}