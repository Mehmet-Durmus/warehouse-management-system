using FluentValidation;
using WHMS.Application.Features.Command.Shipment.UpdateShipment;

namespace WHMS.Application.Validators.Shipment;

public class UpdateShipmentCommandValidator : AbstractValidator<UpdateShipmentCommandRequest>
{
    public UpdateShipmentCommandValidator()
    {
        RuleFor(x => x.ShipmentId)
            .NotEmpty().WithMessage("The DeliveryId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The DeliveryId must be a valid GUID.");

        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("The WarehouseId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The WarehouseId must be a valid GUID.");
        
        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("The StoreId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The StoreId must be a valid GUID.");

        RuleFor(x => x.ExpectedSendingDate)
            .NotNull().WithMessage("Expected sending date is required.")
            .Must(date => date.Date >= DateTime.Now.Date)
            .WithMessage("Expected sending date cannot be in the past.");
    }
}