using FluentValidation;
using WHMS.Application.Features.Queries.StockQuery.GetTotalProductCount;

namespace WHMS.Application.Validators.StockQuery;

public class GetTotalProductCountQueryValidator : AbstractValidator<GetTotalProductCountQueryRequest>
{
    public GetTotalProductCountQueryValidator()
    {
        RuleFor(x => x.WarehouseId)
            .Must(x =>
                string.IsNullOrWhiteSpace(x) ||
                Guid.TryParse(x, out _))
            .WithMessage("The WarehouseId must be a valid GUID or empty.");

        RuleFor(x => x.CityId)
            .Must(x =>
                string.IsNullOrWhiteSpace(x) ||
                Guid.TryParse(x, out _))
            .WithMessage("The CityId must be a valid GUID or empty.");

        RuleFor(x => x.DistrictId)
            .Must(x =>
                string.IsNullOrWhiteSpace(x) ||
                Guid.TryParse(x, out _))
            .WithMessage("The DistrictId must be a valid GUID or empty.");

        RuleFor(x => x.NeighborhodId)
            .Must(x =>
                string.IsNullOrWhiteSpace(x) ||
                Guid.TryParse(x, out _))
            .WithMessage("The NeighborhodId must be a valid GUID or empty.");
    }
}