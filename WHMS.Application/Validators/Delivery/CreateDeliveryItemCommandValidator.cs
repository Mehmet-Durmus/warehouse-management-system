using FluentValidation;
using WHMS.Application.Features.Command.Delivery.CreateDeliveryItem;

namespace WHMS.Application.Validators.Delivery;

public class CreateDeliveryItemCommandValidator : AbstractValidator<CreateDeliveryItemCommandRequest>
{
    public CreateDeliveryItemCommandValidator()
    {
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