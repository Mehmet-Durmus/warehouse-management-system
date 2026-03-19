using FluentValidation;
using WHMS.Application.Features.Command.InventoryCount.CreateInventoryCount;

namespace WHMS.Application.Validators.InventoryCount;

public class CreateInventoryCountCommandValidator : AbstractValidator<CreateInventoryCountCommandRequest>
{
    public CreateInventoryCountCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("The WarehouseId is required")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The WarehouseId must be a valid GUID.");
    }
}