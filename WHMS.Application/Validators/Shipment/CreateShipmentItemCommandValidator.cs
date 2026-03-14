using FluentValidation;
using WHMS.Application.Features.Command.Shipment.CreateShipmentItem;

namespace WHMS.Application.Validators;

public class CreateShipmentItemCommandValidator : AbstractValidator<CreateShipmentItemCommandRequest>
{
    public CreateShipmentItemCommandValidator()
    {
        RuleFor(x => x.ShipmentId)
            .NotEmpty().WithMessage("The ShipmentId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The ShipmentId must be a valid GUID.");
            
        RuleFor(x => x.SkuId)
            .NotEmpty().WithMessage("The SkuId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The SkuId must be a valid GUID.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Delivery item quantity must be greater than 0.");
    }
}