using FluentValidation;
using WHMS.Application.Features.Queries.InventoryCount.GetInventoryCountLine;

namespace WHMS.Application.Validators.InventoryCount;

public class GetInventoryCountLineQueryValidator : AbstractValidator<GetInventoryCountLineQueryRequest>
{
    public GetInventoryCountLineQueryValidator()
    {
        RuleFor(x => x.InventoryCountLineId)
            .NotEmpty().WithMessage("The InventoryCountLineId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The InventoryCountLineId is must be a valid GUID.");
    }
}