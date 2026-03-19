using FluentValidation;
using WHMS.Application.Features.Command.InventoryCount.DeleteInventoryCount;

namespace WHMS.Application.Validators.InventoryCount;

public class DeleteInventoryCountCommandValidator : AbstractValidator<DeleteInventoryCountCommandRequest>
{
    public DeleteInventoryCountCommandValidator()
    {
        RuleFor(x => x.InventoryCountId)
            .NotEmpty().WithMessage("The InventoryCountId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The InventoryCountId must be a valid GUID.");
    }
}