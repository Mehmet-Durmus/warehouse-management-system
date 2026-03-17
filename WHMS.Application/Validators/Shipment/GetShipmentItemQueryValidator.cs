using FluentValidation;
using WHMS.Application.Features.Queries.Shipment.GetShipmentItem;

namespace WHMS.Application.Validators.Shipment;

public class GetShipmentItemQueryValidator : AbstractValidator<GetShipmentItemQueryRequest>
{
    public GetShipmentItemQueryValidator()
    {
        RuleFor(x => x.ShipmentItemId)
            .NotNull().WithMessage("The ShipmentItemId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The ShipmentItemId is must be valid GUID.");
    }
}