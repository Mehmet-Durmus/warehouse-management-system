using FluentValidation;
using WHMS.Application.Features.Command.Warehouse.UpdateWarehouse;

namespace WHMS.Application.Validators.Warehose;

public class UpdateWarehouseCommandValidator : AbstractValidator<UpdateWarehouseCommandRequest>
{
    public UpdateWarehouseCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("Provice a warehouse.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("WarehouseId must be a valid GUID.");
        RuleFor(x => x.CityId)
            .NotEmpty().WithMessage("Provice a city.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("CityId must be a valid GUID.");
        RuleFor(x => x.DistrictId)
            .NotEmpty().WithMessage("Provice a district.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("DistrictId must be a valid GUID.");
        RuleFor(x => x.NeighborhoodId)
            .NotEmpty().WithMessage("Provice a neighborhood.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("NeighborhoodId must be a valid GUID.");
        RuleFor(x => x.AddressLine)
            .NotEmpty().WithMessage("Address line cannot be empty.");
        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Provice a postal code.");
    }
}