using FluentValidation;
using WHMS.Application.Features.Queries.Warehouse.GetWarehouse;

namespace WHMS.Application.Validators.Warehose;

public class GetWarehouseQueryValidator : AbstractValidator<GetWarehouseQueryRequest>
{
    public GetWarehouseQueryValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("Valid a warehouse")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("WarehouseId must be a valid GUID.");
    }
}