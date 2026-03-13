using FluentValidation;
using WHMS.Application.Features.Command.Delivery.DeleteDelivery;

namespace WHMS.Application.Validators;

public class DeleteDeliveryCommandValidator : AbstractValidator<DeleteDeliveryCommandRequest>
{
    public DeleteDeliveryCommandValidator()
    {
        RuleFor(x => x.DeliveryId)
            .NotEmpty().WithMessage("The DeliveryId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The DeliveryId must be a valid GUID.");
    }
}