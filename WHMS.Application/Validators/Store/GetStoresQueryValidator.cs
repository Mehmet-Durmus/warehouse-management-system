using FluentValidation;
using WHMS.Application.Features.Queries.Store.GetStores;

namespace WHMS.Application.Validators.Store;

public class GetStoresQueryValidator : AbstractValidator<GetStoresQueryRequest>
{
    public GetStoresQueryValidator()
    {
        RuleFor(x => x.CityId)
            .Must(x => string.IsNullOrWhiteSpace(x) || Guid.TryParse(x, out _))
            .WithMessage("The CityId must be a valid GUID or empty.");

        RuleFor(x => x.DistrictId)
            .Must(x => string.IsNullOrWhiteSpace(x) || Guid.TryParse(x, out _))
            .WithMessage("The DistrictId must be a valid GUID or empty.");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be a greater than 0.");
    }
}