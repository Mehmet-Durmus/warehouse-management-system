using FluentValidation;
using WHMS.Application.Features.Command.InventoryCount.UpdateInventoryCount;

namespace WHMS.Application.Validators.InventoryCount;

public class UpdateInventoryCountCommandValidator : AbstractValidator<UpdateInventoryCountCommandRequest>
{
    public UpdateInventoryCountCommandValidator()
    {
        RuleFor(x => x.InventoryCountId)
            .NotEmpty().WithMessage("The InventoryCountId is required")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The InventoryCountId must be a valid GUID.");

        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("The WarehouseId is required")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The WarehouseId must be a valid GUID.");
    }
}