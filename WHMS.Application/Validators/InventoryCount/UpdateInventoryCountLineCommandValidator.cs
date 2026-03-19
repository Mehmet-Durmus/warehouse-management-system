using FluentValidation;
using WHMS.Application.Features.Command.InventoryCount.UpdateInventoryCountLine;

namespace WHMS.Application.Validators.InventoryCount;

public class UpdateInventoryCountLineCommandValidator : AbstractValidator<UpdateInventoryCountLineCommandRequest>
{
    public UpdateInventoryCountLineCommandValidator()
    {
        RuleFor(x => x.InventoryCountLineId)
            .NotEmpty().WithMessage("The InventoryCountLineId is required")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The InventoryCountLineId must be a valid GUID.");

        RuleFor(x => x.InventoryCountId)
            .NotEmpty().WithMessage("The InventoryCountId is required")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The InventoryCountId must be a valid GUID.");

        RuleFor(x => x.SkuId)
            .NotEmpty().WithMessage("The SkuId is required")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The SkuId must be a valid GUID.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("The quantity field must be greater than 0.");
    }
}