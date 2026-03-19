using FluentValidation;
using WHMS.Application.Features.Command.InventoryCount.DeleteInventoryCountLine;

namespace WHMS.Application.Validators.InventoryCount;

public class DeleteInventoryCountLineCommandValidator : AbstractValidator<DeleteInventoryCountLineCommandRequest>
{
    public DeleteInventoryCountLineCommandValidator()
    {
        RuleFor(x => x.InventoryCountLineId)
            .NotEmpty().WithMessage("The InventoryCountLineId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The InventoryCountLineId is must be a valid GUID.");
    }
}