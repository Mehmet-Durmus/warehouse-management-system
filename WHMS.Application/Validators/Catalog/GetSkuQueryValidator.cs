using FluentValidation;
using WHMS.Application.Features.Queries.Catalog.GetSku;

namespace WHMS.Application.Validators.Catalog;

public class GetSkuQueryValidator : AbstractValidator<GetSkuQueryRequest>
{
    public GetSkuQueryValidator()
    {
        RuleFor(x => x.SkuId)
            .NotEmpty().WithMessage("The SkuId is required")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The SkuId must be a valid GUID.");
    }
}