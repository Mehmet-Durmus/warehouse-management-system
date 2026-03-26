using System.Data;
using FluentValidation;
using WHMS.Application.Features.Command.InventoryCount.CompleteInventoryCount;

namespace WHMS.Application.Validators.InventoryCount;

public class CompleteInventoryCountCommandValidator : AbstractValidator<CompleteInventoryCountCommandRequest>
{
    public CompleteInventoryCountCommandValidator()
    {
        RuleFor(x => x.InventoryCountId)
            .NotEmpty().WithMessage("The InventoryCountId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The InventoryCountId must be a valid GUID.");
    }
}