using FluentValidation;
using WHMS.Application.Features.Queries.StockQuery.GetProductCounts;

namespace WHMS.Application.Validators.StockQuery;

public class GetProductCountsQueryValidator : AbstractValidator<GetProductCountsQueryRequest>
{
    public GetProductCountsQueryValidator()
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