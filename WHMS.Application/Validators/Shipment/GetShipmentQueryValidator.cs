using FluentValidation;
using WHMS.Application.Features.Queries.Shipment.GetShipment;

namespace WHMS.Application.Validators.Shipment;

public class GetShipmentQueryValidator : AbstractValidator<GetShipmentQueryRequest>
{
    public GetShipmentQueryValidator()
    {
        RuleFor(x => x.ShipmentId)
            .NotNull().WithMessage("The ShipmentId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The ShipmentId must be a valid GUID.");
    }
}