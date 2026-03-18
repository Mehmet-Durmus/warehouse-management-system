using FluentValidation;
using WHMS.Application.Features.Command.Shipment.UpdateShipmentItem;

namespace WHMS.Application.Validators.Shipment;

public class UpdateShipmentItemCommandValidator : AbstractValidator<UpdateShipmentItemCommandRequest>
{
    public UpdateShipmentItemCommandValidator()
    {
        RuleFor(x => x.ShipmentItemId)
            .NotEmpty().WithMessage("The ShipmentItemId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The ShipmentItemId must be a valid GUID.");

        RuleFor(x => x.ShipmentId)
            .NotEmpty().WithMessage("The ShipmentId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The ShipmentId must be a valid GUID.");

        RuleFor(x => x.SkuId)
            .NotEmpty().WithMessage("The SkuId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The SkuId must be a valid GUID.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Shipment item quantity must be greater than 0.");
    }
}