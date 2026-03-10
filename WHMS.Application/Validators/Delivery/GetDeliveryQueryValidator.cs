using FluentValidation;
using WHMS.Application.Features.Queries.Delivery.GetDelivery;

namespace WHMS.Application.Validators.Delivery;

public class GetDeliveryQueryValidator : AbstractValidator<GetDeliveryQueryRequet>
{
    public GetDeliveryQueryValidator()
    {
        RuleFor(x => x.DeliveryId)
            .NotEmpty().WithMessage("The DeliveryId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The DeliveryId must be a valid GUID.");
    }
}