using FluentValidation;
using WHMS.Application.Features.Queries.Shipment.GetShipments;

namespace WHMS.Application.Validators.Shipment;

public class GetShipmentsQueryValidator : AbstractValidator<GetShipmentsQueryRequest>
{
    public GetShipmentsQueryValidator()
    {
        RuleFor(x => x.WarehouseId)
            .Must(x => string.IsNullOrWhiteSpace(x) || Guid.TryParse(x, out _))
            .WithMessage("The WarehouseId must be a valid GUID or empty.");

        RuleFor(x => x.WarehouseCityId)
            .Must(x => string.IsNullOrWhiteSpace(x) || Guid.TryParse(x, out _))
            .WithMessage("The WarehouseCityId must be a valid GUID or empty.");

        RuleFor(x => x.WarehouseDistrictId)
            .Must(x => string.IsNullOrWhiteSpace(x) || Guid.TryParse(x, out _))
            .WithMessage("The WarehouseDistrictId must be a valid GUID or empty.");

        RuleFor(x => x.StoreId)
            .Must(x => string.IsNullOrWhiteSpace(x) || Guid.TryParse(x, out _))
            .WithMessage("The StoreId must be a valid GUID or empty.");

        RuleFor(x => x.StoreCityId)
            .Must(x => string.IsNullOrWhiteSpace(x) || Guid.TryParse(x, out _))
            .WithMessage("The StoreCityId must be a valid GUID or empty.");

        RuleFor(x => x.StoreCityId)
            .Must(x => string.IsNullOrWhiteSpace(x) || Guid.TryParse(x, out _))
            .WithMessage("The StoreCityId must be a valid GUID or empty.");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be a greater than 0.");
    }
}