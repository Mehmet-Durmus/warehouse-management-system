using FluentValidation;
using WHMS.Application.Features.Command.Shipment.DeleteShipmentItem;

namespace WHMS.Application.Validators.Shipment;

public class DeleteShipmentItemCommandValidator : AbstractValidator<DeleteShipmentItemCommandRequest>
{
    public DeleteShipmentItemCommandValidator()
    {
        RuleFor(x => x.ShipmentItemId)
            .NotEmpty().WithMessage("The ShipmentItemId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The ShipmentItemId must be a valid GUID.");
    }
}