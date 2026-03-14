using FluentValidation;
using WHMS.Application.Features.Command.Delivery.DeleteDeliveryItem;

namespace WHMS.Application.Validators;

public class DeleteDeliveryItemCommandValidator : AbstractValidator<DeleteDeliveryItemCommandRequest>
{
    public DeleteDeliveryItemCommandValidator()
    {
        RuleFor(x => x.DeliveryItemId)
            .NotEmpty().WithMessage("The DeliveryItemId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The DeliveryItemId must be a valid GUID.");
    }
}