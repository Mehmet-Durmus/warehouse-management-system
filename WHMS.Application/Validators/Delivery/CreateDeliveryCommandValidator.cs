using FluentValidation;
using WHMS.Application.Features.Command.Delivery.CreateDelivery;

namespace WHMS.Application.Validators.Delivery;

public class CreateDeliveryCommandValidator : AbstractValidator<CreateDeliveryCommandRequest>
{
    public CreateDeliveryCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("The WarehouseId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The WarehouseId must be a valid GUID.");

        RuleFor(x => x.ExpectedArrivalDate)
            .NotNull().WithMessage("Expected arrival date is required.")
            .Must(date => date.Date >= DateTime.Now.Date)
            .WithMessage("Expected arrival date cannot be in the past.");
    }
}