using FluentValidation;
using WHMS.Application.Features.Command.Warehouse.CreateWarehouse;

namespace WHMS.Application.Validators.Warehose;

public class CreateWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommandRequest>
{
    public CreateWarehouseCommandValidator()
    {
        RuleFor(x => x.CityId)
            .NotEmpty().WithMessage("Provice a city.");
        RuleFor(x => x.DistrictId)
            .NotEmpty().WithMessage("Provice a district.");
        RuleFor(x => x.NeighborhoodId)
            .NotEmpty().WithMessage("Provice a neighborhood.");
        RuleFor(x => x.AddressLine)
            .NotEmpty().WithMessage("Address line cannot be empty.");
        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Provice a postal code.");
    }
}