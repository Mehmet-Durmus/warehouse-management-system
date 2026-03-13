using FluentValidation;
using WHMS.Application.Features.Command.Delivery.UpdateDeliveryItem;

namespace WHMS.Application.Validators;

public class UpdateDeliveryItemCommandValidator : AbstractValidator<UpdateDeliveryItemCommandRequest>
{
    public UpdateDeliveryItemCommandValidator()
    {
        RuleFor(x => x.DeliveryItemId)
            .NotEmpty().WithMessage("The DeliveryItemId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The DeliveryItemId must be a valid GUID.");

        RuleFor(x => x.DeliveryId)
            .NotEmpty().WithMessage("The DeliveryId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The DeliveryId must be a valid GUID.");

        RuleFor(x => x.SkuId)
            .NotEmpty().WithMessage("The SkuId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The SkuId must be a valid GUID.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Delivery item quantity must be greater than 0.");
    }
}