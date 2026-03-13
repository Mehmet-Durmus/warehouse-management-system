using FluentValidation;
using WHMS.Application.Features.Queries.GetDeliveryItem;

namespace WHMS.Application.Validators;

public class GetDeliveryItemQueryValidator : AbstractValidator<GetDeliveryItemQueryRequest>
{
    public GetDeliveryItemQueryValidator()
    {
        RuleFor(x => x.DeliveryItemId)
            .NotEmpty().WithMessage("The DeliveryItemId is required")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The DeliveryItemId must be a valid GUID.");
    }
}