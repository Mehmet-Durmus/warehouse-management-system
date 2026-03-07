using FluentValidation;
using WHMS.Application.Features.Command.Warehouse.CreateWarehouse;

namespace WHMS.Application.Validators.Warehose;

public class CreateWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommandRequest>
{
    public CreateWarehouseCommandValidator()
    {
        RuleFor(x => x.CityId)
            .NotEmpty().WithMessage("Provide a city.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("CityId must be a valid GUID.");
        RuleFor(x => x.DistrictId)
            .NotEmpty().WithMessage("Provide a district.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("DistrictId must be a valid GUID.");
        RuleFor(x => x.NeighborhoodId)
            .NotEmpty().WithMessage("Provide a neighborhood.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("NeighborhoodId must be a valid GUID.");
        RuleFor(x => x.AddressLine)
            .NotEmpty().WithMessage("Address line cannot be empty.");
        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Provide a postal code.");
    }
}