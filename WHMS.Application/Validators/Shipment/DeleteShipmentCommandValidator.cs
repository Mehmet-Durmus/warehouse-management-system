using FluentValidation;
using WHMS.Application.Features.Command.Shipment.DeleteShipment;

namespace WHMS.Application.Validators.Shipment;

public class DeleteShipmentCommandValidator : AbstractValidator<DeleteShipmentCommandRequest>
{
    public DeleteShipmentCommandValidator()
    {
        RuleFor(x => x.ShipmentId)
            .NotEmpty().WithMessage("The ShipmentId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The ShipmentId must be a valid GUID.");
    }
}